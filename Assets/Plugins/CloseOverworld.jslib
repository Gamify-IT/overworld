mergeInto(LibraryManager.library, {
  CloseOverworld: function () {
    window.parent.postMessage("CLOSE ME");
  },
});
