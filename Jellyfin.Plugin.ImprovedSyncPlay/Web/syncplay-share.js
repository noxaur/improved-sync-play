(function () {
    'use strict';

    var SHARE_ITEM_ID = 'improved-syncplay-share';

    function getApiClient() {
        if (window.ApiClient) {
            return window.ApiClient;
        }

        if (window.connectionManager && typeof window.connectionManager.currentApiClient === 'function') {
            return window.connectionManager.currentApiClient();
        }

        return null;
    }

    function getSyncPlayManager() {
        return window.SyncPlay && window.SyncPlay.Manager ? window.SyncPlay.Manager : null;
    }

    function getGroupIdFromManager() {
        var manager = getSyncPlayManager();
        if (!manager || typeof manager.getGroupInfo !== 'function') {
            return null;
        }

        var info = manager.getGroupInfo();
        return info && (info.GroupId || info.groupId) || null;
    }

    function getActiveGroupId() {
        return Promise.resolve(getGroupIdFromManager() || null);
    }

    function buildShareUrl(groupId) {
        return window.location.origin + '/web/?syncplayGroup=' + encodeURIComponent(groupId);
    }

    function copyShareUrl(url) {
        if (navigator.clipboard && typeof navigator.clipboard.writeText === 'function') {
            return navigator.clipboard.writeText(url).catch(function () {
                window.prompt('Copy SyncPlay invite link:', url);
            });
        }

        window.prompt('Copy SyncPlay invite link:', url);
        return Promise.resolve();
    }

    function isActiveSyncPlaySession() {
        return !!document.querySelector('#sync-play-active-subheader');
    }

    function createShareMenuItem() {
        var item = document.createElement('li');
        item.id = SHARE_ITEM_ID;
        item.setAttribute('role', 'menuitem');
        item.className = 'MuiMenuItem-root MuiMenuItem-gutters btnShareSyncPlay';
        item.tabIndex = -1;
        item.textContent = 'Share';
        item.addEventListener('click', function (event) {
            event.preventDefault();
            event.stopPropagation();

            getActiveGroupId().then(function (groupId) {
                if (!groupId) {
                    console.warn('[ImprovedSyncPlay] No active SyncPlay group to share');
                    return;
                }

                return copyShareUrl(buildShareUrl(groupId));
            });
        });

        return item;
    }

    function injectShareMenuItem(menuList) {
        if (!menuList || document.getElementById(SHARE_ITEM_ID) || !isActiveSyncPlaySession()) {
            return;
        }

        menuList.appendChild(createShareMenuItem());
    }

    function tryInjectShareMenu() {
        var menuList = document.querySelector('#app-sync-play-menu [role="menu"]');
        if (menuList) {
            injectShareMenuItem(menuList);
        }
    }

    function whenApiClientReady(callback, attemptsLeft) {
        var api = getApiClient();
        if (api) {
            callback(api);
            return;
        }

        if (attemptsLeft <= 0) {
            return;
        }

        window.setTimeout(function () {
            whenApiClientReady(callback, attemptsLeft - 1);
        }, 250);
    }

    function joinFromQueryParam() {
        var params = new URLSearchParams(window.location.search);
        var groupId = params.get('syncplayGroup');
        if (!groupId) {
            return;
        }

        whenApiClientReady(function (api) {
            var joinPromise;
            if (typeof api.joinSyncPlayGroup === 'function') {
                joinPromise = api.joinSyncPlayGroup({ GroupId: groupId });
            } else {
                joinPromise = api.ajax({
                    type: 'POST',
                    url: api.getUrl('SyncPlay/Join'),
                    data: JSON.stringify({ GroupId: groupId }),
                    contentType: 'application/json'
                });
            }

            joinPromise.then(function () {
                params.delete('syncplayGroup');
                var query = params.toString();
                var cleanUrl = window.location.pathname + (query ? '?' + query : '') + window.location.hash;
                window.history.replaceState({}, '', cleanUrl);
            }).catch(function (error) {
                console.warn('[ImprovedSyncPlay] Failed to join SyncPlay group from link', error);
            });
        }, 40);
    }

    function observeSyncPlayMenus() {
        if (!document.body) {
            return;
        }

        var observer = new MutationObserver(function () {
            tryInjectShareMenu();
        });

        observer.observe(document.body, { childList: true, subtree: true });
    }

    function initialize() {
        tryInjectShareMenu();
        joinFromQueryParam();
        observeSyncPlayMenus();
    }

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', initialize);
    } else {
        initialize();
    }
})();
