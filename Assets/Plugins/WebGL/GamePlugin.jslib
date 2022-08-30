mergeInto(LibraryManager.library, {
    OnUnityLoadProgress: function (progress) {
        try {
            window.dispatchReactUnityEvent("OnUnityLoadProgress", Pointer_stringify(progress));
        } catch (e) {
            console.warn("Failed to dispatch event OnUnityLoadProgress");
        }
    },
	OnUnityLoaded: function (metadata) {
		try {
			window.dispatchReactUnityEvent("OnUnityLoaded", Pointer_stringify(metadata));
		} catch (e) {
			console.warn("Failed to dispatch event OnUnityLoaded");
		}
	},
	SendCommand: function (command) {
		try {
			window.dispatchReactUnityEvent("SendCommand", Pointer_stringify(command));
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
			window.dispatchReactUnityEvent("OpenLinkInNewTab", Pointer_stringify(link));
		} catch (e) {
			console.warn("Failed to dispatch event OpenLinkInNewTab");
		}
	},
});
