import './main.css';
import * as monaco from 'monaco-editor';
import { inflate, deflate } from 'pako';
import exampleFile from '../../../docs/examples/101/1vm-2nics-2subnets-1vnet/main.bicep';
var buildVersion = require('../package.json').version;

document.title = `${document.title} ${buildVersion}`;
document.body.removeAttribute('style');

let initialValue = exampleFile;
try {
  const rawHash = window.location.hash.substr(1);
  history.replaceState(null, null, ' ');
  const hashContents = inflate(atob(rawHash), { to: 'string' });
  if (hashContents) {
    initialValue = hashContents;
  }
} catch {
}

self['MonacoEnvironment'] = {
  getWorkerUrl: function (moduleId, label) {
    if (label === 'json') {
      return './json.worker.bundle.js';
    }
    return './editor.worker.bundle.js';
  }
}

self['BicepInitialize'] = (interop) => {
  monaco.languages.register({
    id: 'bicep',
    extensions: ['.bicep'],
    aliases: ['bicep'],
  });

  monaco.languages.registerDocumentSemanticTokensProvider('bicep', {
    getLegend: () => interop.invokeMethod('GetSemanticTokensLegend'),
    provideDocumentSemanticTokens: (model, lastResultId, token) => interop.invokeMethod('GetSemanticTokens', model.getValue()),
    releaseDocumentSemanticTokens: () => { }
  });

  const editorLhs = monaco.editor.create(document.getElementById('editor-lhs'), {
    theme: 'vs-dark',
    automaticLayout: true,
    language: 'bicep',
    minimap: {
      enabled: false,
    },
    value: initialValue,
    // @ts-expect-error
    'semanticHighlighting.enabled': true
  });

  // @ts-expect-error
  editorLhs._themeService._theme.getTokenStyleMetadata = (type, modifiers) => {
    switch (type) {
      case 'keyword':
        return { foreground: 12 };
      case 'comment':
        return { foreground: 7 };
      case 'parameter':
        return { foreground: 2 };
      case 'property':
        return { foreground: 3 };
      case 'type':
        return { foreground: 8 };
      case 'member':
        return { foreground: 6 };
      case 'string':
        return { foreground: 5 };
      case 'variable':
        return { foreground: 4 };
      case 'operator':
        return { foreground: 9 };
      case 'function':
        return { foreground: 13 };
      case 'number':
        return { foreground: 15 };
      case 'class':
      case 'enummember':
      case 'event':
      case 'modifier':
      case 'label':
      case 'typeParameter':
      case 'macro':
      case 'interface':
      case 'enum':
      case 'regexp':
      case 'struct':
      case 'namespace':
        return { foreground: 0 };
    }
  };

  const editorRhs = monaco.editor.create(document.getElementById('editor-rhs'), {
    theme: 'vs-dark',
    automaticLayout: true,
    language: 'json',
    minimap: {
      enabled: false,
    },
    readOnly: true,
  });

  function compileAndEmitDiagnostics() {
    const bicepModel = editorLhs.getModel();
    const { template, diagnostics } = interop.invokeMethod('CompileAndEmitDiagnostics', bicepModel.getValue());

    editorRhs.setValue(template);
    monaco.editor.setModelMarkers(bicepModel, 'default', diagnostics);
  }

  editorLhs.onDidChangeModelContent(e => compileAndEmitDiagnostics());
  compileAndEmitDiagnostics();

  setupCopyHandler(
    document.getElementById('btn-share'),
    () => editorLhs.getModel().getValue()
  );

  document.getElementById('loader').classList.add('hidden');
  document.getElementById('container').classList.remove('hidden');
}

function setupCopyHandler(shareButton: HTMLElement, getContent: () => string) {
  const shareText = shareButton.textContent;
  shareButton.addEventListener('click', () => {
    document.addEventListener('copy', function onCopy(e: ClipboardEvent) {
      const contentHash = btoa(deflate(getContent(), { to: 'string' }));
      e.clipboardData.setData('text/plain', `https://aka.ms/bicepdemo#${contentHash}`);
      e.preventDefault();
      document.removeEventListener('copy', onCopy, true);
    });

    document.execCommand('copy');

    setTimeout(() => shareButton.textContent = shareText, 2000);
    shareButton.textContent = 'copied!'
  });
}