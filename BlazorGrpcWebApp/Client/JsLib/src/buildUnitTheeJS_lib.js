import * as THREE from 'three';
import { FBXLoader } from 'three/examples/jsm/loaders/FBXLoader'
//import * from '../../wwwroot/models'

let scene, container, renderer, camera;

const params = {
    color: '#ffffff'
};

const setupScene = function () {
    scene = new THREE.Scene();
    scene.background = new THREE.Color(params.color);

    const light = new THREE.PointLight()
    light.position.set(0.8, 1.4, 1.0)
    scene.add(light)

    const ambientLight = new THREE.AmbientLight()
    scene.add(ambientLight)
}

const setupCamera = function () {
    camera = new THREE.PerspectiveCamera(75, container.clientWidth / container.clientHeight, 0.1, 1000);
    camera.position.set(0.8, 1.4, 1.0);

    window.addEventListener('resize', () => {
        camera.aspect = container.innerWidth / container.innerHeight;
        camera.updateProjectionMatrix();
        renderer.setSize(container.innerWidth, container.innerHeight);
        renderer.render(scene, camera);
    }, false);
}

const setupRenderer = function () {
    renderer = new THREE.WebGLRenderer({ alpha: true, antialias: true });
    renderer.setPixelRatio(window.devicePixelRatio);
    renderer.setSize(container.clientWidth, container.clientHeight);
    renderer.autoClear = false;
    renderer.outputEncoding = THREE.sRGBEncoding;
    renderer.toneMapping = THREE.ACESFilmicToneMapping;
    renderer.toneMappingExposure = 1.0;
}

const animate = function () {
    requestAnimationFrame(animate);
    renderer.render(scene, camera);
}

const appendScene = function () {
    while (container.lastElementChild) {
        container.removeChild(container.lastElementChild);
    }

    container.appendChild(renderer.domElement);
}

export function loadScene() {
    container = document.getElementById('BuildThreeJSContainer');
    if (typeof container == 'undefined' || container == null) {
        return;
    }

    setupScene();
    setupCamera();
    scene.add(camera);

    const fbxLoader = new FBXLoader();
    fbxLoader.load('')


    setupRenderer();
    animate();
    appendScene();

    console.log("TreeJS successfully loaded.");
}