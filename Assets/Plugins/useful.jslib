mergeInto(LibraryManager.library, {
  IsMobileBrowser: function () {
    return (/iPhone|iPad|iPod|Android/i.test(navigator.userAgent));
  },
  
  ShowAlert: function(str) {
	window.alert(Pointer_stringify(str));
  },
  
  AllowCursorLock: function() {
    allowLock = true;
  },

  DisallowCursorLock: function() {
    allowLock = false;
  },

  _UnlockCursor: function() {
    document.exitPointerLock();
  },

  ReadCookies: function (name) {
    var returnStr=""
	const value = "; " + decodeURIComponent(document.cookie);
	const parts = value.split("; " + Pointer_stringify(name));
	if (parts.length === 2)
    returnStr =  parts.pop().split(";").shift().substring(1);
    var bufferSize = lengthBytesUTF8(returnStr) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(returnStr, buffer, bufferSize);
    return buffer;
  },

  ReadLocalStorage: function (name) {
    var returnStr = localStorage.getItem(Pointer_stringify(name));
    if (returnStr === null)
      returnStr = ""
    var bufferSize = lengthBytesUTF8(returnStr) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(returnStr, buffer, bufferSize);
    return buffer;
  },

  ReadRemoteHostURL: function () {
    var returnStr = (typeof REMOTE_HOST_URL !== "undefined") ? REMOTE_HOST_URL : "twnz.dev";
    if (returnStr === null)
      returnStr = ""
    var bufferSize = lengthBytesUTF8(returnStr) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(returnStr, buffer, bufferSize);
    return buffer;
  },

  PrintFloatArray: function (array, size) {
    for(var i = 0; i < size; i++)
    console.log(HEAPF32[(array >> 2) + i]);
  },

  AddNumbers: function (x, y) {
    return x + y;
  },

  StringReturnValueFunction: function () {
    var returnStr = "bla";
    var bufferSize = lengthBytesUTF8(returnStr) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(returnStr, buffer, bufferSize);
    return buffer;
  },

  BindWebGLTexture: function (texture) {
    GLctx.bindTexture(GLctx.TEXTURE_2D, GL.textures[texture]);
  },

});