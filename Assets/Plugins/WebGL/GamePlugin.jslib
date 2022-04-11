mergeInto(LibraryManager.library, {
    OnUnityLoaded: function (message) {
        ReactUnityWebGL.OnUnityLoaded();
    },
    OnGameOverPopupButtonClicked: function () {
        ReactUnityWebGL.OnGameOverPopupButtonClicked();
    },
    OpenLinkInNewTab: function (link) {
        ReactUnityWebGL.OpenLinkInNewTab(Pointer_stringify(link));
    },
    SendCommand: function (command) {
        ReactUnityWebGL.SendCommand(Pointer_stringify(command));
    }
});