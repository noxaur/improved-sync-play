(function () {
    'use strict';

    var SHARE_ITEM_ID = 'improved-syncplay-share';

    function getApiClient() {
        return window.ApiClient;
    }

    function getSyncPlayManager() {
        return window.SyncPlay && window.SyncPlay.Manager && window.SyncPlay.Manager.getInstance
            ? window.SyncPlay.Manager.getInstance()
            : null;
    }

    function getGroupIdFromManager() {
        var manager = getSyncPlayManager();
        if (!manager) {
            return null;
        }

        if (typeof manager.getCurrentGroupId === 'function') {
            return manager.getCurrentGroupId();
        }

        return manager.currentGroupId || manager.groupId || null;
    }

    function listSyncPlayGroups() {
        var api = getApiClient();
        if (!api) {
            return Promise.resolve([]);
        }

        return api.ajax({
            type: 'GET',
            url: api.getUrl('SyncPlay/List')
        }).then(function (groups) {
            return groups || [];
        }).catch(function (error) {
            console.warn('[ImprovedSyncPlay] Failed to list SyncPlay groups', error);
            return [];
        });
    }

    function getActiveGroupId() {
        var managerGroupId = getGroupIdFromManager();
        if (managerGroupId) {
            return Promise.resolve(managerGroupId);
        }

        return listSyncPlayGroups().then(function (groups) {
            var active = groups.find(function (group) {
                return group.IsActive || (group.Participants && group.Participants.length > 0);
            });

            return (active && active.GroupId) || (groups[0] && groups[0].GroupId) || null;
        });
    }

    function buildShareUrl(groupId) {
        return window.location.origin + window.location.pathname + '?syncplayGroup=' + encodeURIComponent(groupId);
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
        var item = document.createElement('button');
        item.id = SHARE_ITEM_ID;
        item.type = 'button';
        item.className = 'menuOption btnShareSyncPlay';
        item.innerHTML = '<span class="material-icons share" aria-hidden="true"></span><span>Share</span>';
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

    function injectShareMenuItem(menu) {
        if (!menu || document.getElementById(SHARE_ITEM_ID) || !isActiveSyncPlaySession()) {
            return;
        }

        menu.appendChild(createShareMenuItem());
    }

    function tryInjectShareMenu() {
        var menu = document.querySelector('#app-sync-play-menu');
        if (menu) {
            injectShareMenuItem(menu);
        }
    }

    function joinFromQueryParam() {
        var params = new URLSearchParams(window.location.search);
        var groupId = params.get('syncplayGroup');
        if (!groupId) {
            return;
        }

        var api = getApiClient();
        if (!api) {
            return;
        }

        api.ajax({
            type: 'POST',
            url: api.getUrl('SyncPlay/Join'),
            data: JSON.stringify({ GroupId: groupId }),
            contentType: 'application/json'
        }).then(function () {
            params.delete('syncplayGroup');
            var query = params.toString();
            var cleanUrl = window.location.pathname + (query ? '?' + query : '') + window.location.hash;
            window.history.replaceState({}, '', cleanUrl);
        }).catch(function (error) {
            console.warn('[ImprovedSyncPlay] Failed to join SyncPlay group from link', error);
        });
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
