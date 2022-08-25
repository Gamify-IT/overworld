mergeInto(LibraryManager.library, {
	LoadMinigameInIframe: function(minigameName, configurationString) {
        	var iframe = document.createElement("iframe");
        	iframe.src = location.origin + "/minigames/"+ UTF8ToString(minigameName).toLowerCase() +"#"+ UTF8ToString(configurationString);
 		iframe.style.position = "fixed";
        	iframe.style.top = "0";
        	iframe.style.left = "0";
        	iframe.style.width = "100%";
        	iframe.style.height = "100%";
        	document.body.appendChild(iframe);

        	window.addEventListener("message", (event) => {
            	// FIXME: validate event origin

            		if (event.data === "CLOSE ME") {
                		iframe.remove();
            		}
        	})
    	}
});
