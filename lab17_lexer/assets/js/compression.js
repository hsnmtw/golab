
const source = document.querySelector('textarea[name="source"]');
const target = document.querySelector('textarea[name="target"]');
const decoded = document.querySelector('textarea[name="decoded"]');
const stats = document.querySelector('#stats');

import('./b64.js').then(module => { 
    window.b64Encode = module.encode;
    window.b64Decode = module.decode;
});

import('./lib/lz-string.js').then(module => {
    window.lzString = module.LZString144;
    window.compressToArray = module.compressToArray;
    window.decompressFromArray = module.decompressFromArray;
})

function updateTarget() {
    target.value = compressToArray(source.value);
    decoded.value = decompressFromArray(JSON.parse(`[${target.value}]`));
    stats.innerHTML = `Source = ${source.value.length} / Target = ${target.value.length}`
}

source.addEventListener("input", updateTarget);

function compress(someText) {
    return (lzString.compress(someText));
}

function decompress(someText) {
    return lzString.decompress((someText));
}