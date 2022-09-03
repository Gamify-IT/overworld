mergeInto(LibraryManager.library, {
	GetToken: function (str0) {
    		var token1 = localStorage[UTF8ToString(str0)];
   
   		if(token1 == 'undefined' || token1 == undefined || token1 == null){
			token1 = '';
		}
   
    		var len = lengthBytesUTF8(token1) + 1;
    		var buffer = _malloc(len);
		
    		stringToUTF8(token1, buffer, len);
 
    		return buffer;
  	}
});
