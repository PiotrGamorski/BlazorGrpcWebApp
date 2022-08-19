import * as THREE from '../build/three.module.js';
import { OrbitControls } from './OrbitControls.js';

let scene, container, renderer, camera, controls;

function animate() {
    controls.update();
    requestAnimationFrame(animate);
    renderer.render(scene, camera);
}

function loadScene() {
    container = document.getElementById('threejscontainer');
    if (!container) {
        return;
    }

    scene = new THREE.Scene();

    const fov = 60;
    const aspect = container.clientWidth / container.clientHeight;
    const near = 0.1;
    const far = 1000;

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

    controls = new OrbitControls(camera, renderer.domElement);
    controls.autoRotate = true;
    controls.autoRotateSpeed = 0.25;
    controls.enableDamping = true;

    const earthGeometry = new THREE.SphereGeometry(0.6, 32, 32);
    const earthMaterial = new THREE.MeshPhongMaterial({
        roughness: 1,
        metalness: 0,
        map: THREE.ImageUtils.loadTexture('textures/earthmap1k.jpg'),
        bumpMap: THREE.ImageUtils.loadTexture('textures/earthbump.jpg'),
        bumpScale: 0.3   
    });
    const earthMesh = new THREE.Mesh(earthGeometry, earthMaterial);
    earthMesh.rotateZ(-0.3);
    scene.add(earthMesh);

    const cloudGeometry = new THREE.SphereGeometry(0.63, 32, 32);
    const cloudMaterial = new THREE.MeshPhongMaterial({
        map: THREE.ImageUtils.loadTexture('textures/earthCloud.png'),
        transparent: true,
    });
    const cloundMesh = new THREE.Mesh(cloudGeometry, cloudMaterial);
    scene.add(cloundMesh);

    const ambientLight = new THREE.AmbientLight(0xffffff, 0.01);
    const pointLight = new THREE.PointLight(0xffffff, 0.3);
    pointLight.position.set(5,3,5);
    scene.add(ambientLight);
    scene.add(pointLight);

    animate();

    while (container.lastElementChild) {
        container.removeChild(container.lastElementChild);
    }

    container.appendChild(renderer.domElement);
}


window.Earth = {
    load: () => { loadScene(); },
};

window.onload = loadScene;