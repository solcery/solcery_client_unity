mergeInto(LibraryManager.library, {
    OnUnityLoadProgress: function (progress) {
        try {
            window.dispatchReactUnityEvent("OnUnityLoadProgress", UTF8ToString(progress));
        } catch (e) {
            console.warn("Failed to dispatch event OnUnityLoadProgress");
        }
    },
	OnUnityLoaded: function (metadata) {
		try {
			window.dispatchReactUnityEvent("OnUnityLoaded", UTF8ToString(metadata));
		} catch (e) {
			console.warn("Failed to dispatch event OnUnityLoaded");
		}
	},
	SendCommand: function (command) {
		try {
			window.dispatchReactUnityEvent("SendCommand", UTF8ToString(command));
		} catch (e) {
			console.warn("Failed to dispatch event SendCommand");
		}
	},
	OnGameOverPopupButtonClicked: function () {
		try {
			window.dispatchReactUnityEvent("OnGameOverPopupButtonClicked");
		} catch (e) {
			console.warn("Failed to dispatch event OnGameOverPopupButtonClicked");
		}
	},
	OpenLinkInNewTab: function (link) {
		try {
			window.dispatchReactUnityEvent("OpenLinkInNewTab", UTF8ToString(link));
		} catch (e) {
			console.warn("Failed to dispatch event OpenLinkInNewTab");
		}
	},
	SyncFiles: function () {
         FS.syncfs(false,function (err) {
             // handle callback
         });
    }
});
