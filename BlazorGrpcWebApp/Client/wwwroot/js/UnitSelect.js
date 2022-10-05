import * as THREE from '../build/three.module.js';

let scene, container, renderer, camera;

function animate() {
    requestAnimationFrame(animate);
    renderer.render(scene, camera);
}

function loadScene() {
    container = document.getElementById('UnitSelectThreeJSContainer');
    if (!container) {
        return;
    }

    scene = new THREE.Scene();

    const fov = 60;
    const aspect = container.clientWidth / container.clientHeight;
    const near = 0.1;
    const far = 10;

    camera = new THREE.PerspectiveCamera(fov, aspect, near, far);
    camera.position.set(2,0,0);
    scene.add(camera);

    renderer = new THREE.WebGLRenderer({ alpha: true, antialias: true });
    renderer.setPixelRatio(window.devicePixelRatio);
    renderer.setSize(container.clientWidth, container.clientHeight);
    renderer.autoClear = false;
    renderer.outputEncoding = THREE.sRGBEncoding;
    renderer.toneMapping = THREE.ACESFilmicToneMapping;
    renderer.toneMappingExposure = 1.0;

    animate();

    while (container.lastElementChild) {
        container.removeChild(container.lastElementChild);
    }

    container.appendChild(renderer.domElement);
}

window.UnitSelect = {
    load: () => { loadScene(); },
};

window.onload = loadScene;