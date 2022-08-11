mergeInto(LibraryManager.library, {
	OnUnityLoaded: function () {
		try {
			window.dispatchReactUnityEvent("OnUnityLoaded");
		} catch (e) {
			console.warn("Failed to dispatch event");
		}
	},
	SendCommand: function (command) {
		try {
			window.dispatchReactUnityEvent("SendCommand", Pointer_stringify(command));
		} catch (e) {
			console.warn("Failed to dispatch event");
		}
	},
	OnGameOverPopupButtonClicked: function () {
		try {
			window.dispatchReactUnityEvent("OnGameOverPopupButtonClicked");
		} catch (e) {
			console.warn("Failed to dispatch event");
		}
	},
	OpenLinkInNewTab: function (link) {
		try {
			window.dispatchReactUnityEvent("OpenLinkInNewTab", Pointer_stringify(link));
		} catch (e) {
			console.warn("Failed to dispatch event");
		}
	},
});
