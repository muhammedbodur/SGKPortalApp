(function () {
  function ensureQuill() {
    if (!window.Quill) {
      throw new Error('Quill not loaded');
    }
  }

  function init(editorSelector, toolbarSelector, initialHtml, dotNetRef, maxChars) {
    ensureQuill();
    var editorEl = document.querySelector(editorSelector);
    if (!editorEl) return null;

    if (editorEl.__sgkQuill) {
      return editorEl.__sgkQuill;
    }

    var options = {
      theme: 'snow',
      modules: {}
    };

    if (toolbarSelector) {
      options.modules.toolbar = toolbarSelector;
    }

    var quill = new Quill(editorEl, options);

    if (initialHtml) {
      quill.clipboard.dangerouslyPasteHTML(initialHtml);
    }

    var lastValidDelta = quill.getContents();

    quill.on('text-change', function () {
      var textLen = quill.getLength() - 1;
      if (maxChars && textLen > maxChars) {
        quill.setContents(lastValidDelta);
        return;
      }

      lastValidDelta = quill.getContents();

      if (dotNetRef) {
        var html = editorEl.querySelector('.ql-editor').innerHTML;
        dotNetRef.invokeMethodAsync('OnQuillContentChanged', html, textLen);
      }
    });

    editorEl.__sgkQuill = quill;
    return quill;
  }

  function setHtml(editorSelector, html) {
    var editorEl = document.querySelector(editorSelector);
    if (!editorEl || !editorEl.__sgkQuill) return;
    editorEl.__sgkQuill.clipboard.dangerouslyPasteHTML(html || '');
  }

  function getHtml(editorSelector) {
    var editorEl = document.querySelector(editorSelector);
    if (!editorEl) return '';
    var ql = editorEl.__sgkQuill;
    if (!ql) {
      var e = editorEl.querySelector('.ql-editor');
      return e ? e.innerHTML : '';
    }
    var ed = editorEl.querySelector('.ql-editor');
    return ed ? ed.innerHTML : '';
  }

  function destroy(editorSelector) {
    var editorEl = document.querySelector(editorSelector);
    if (!editorEl) return;
    if (editorEl.__sgkQuill) {
      delete editorEl.__sgkQuill;
    }
    editorEl.innerHTML = '';
  }

  window.sgkQuill = {
    init: init,
    setHtml: setHtml,
    getHtml: getHtml,
    destroy: destroy
  };
})();
