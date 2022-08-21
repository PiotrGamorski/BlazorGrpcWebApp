import * as THREE from '../build/three.module.js';
import { OrbitControls } from './OrbitControls.js';
import { TWEEN } from '../build/tween.js'

let scene, container, renderer, camera, controls;
let earthMesh, cloudMesh;
let tweenCompleted = false;

function animate() {
    if (tweenCompleted) {
        controls.update();
    }

    requestAnimationFrame(animate);
    TWEEN.update();
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

    controls = new OrbitControls(camera, renderer.domElement);
    controls.autoRotate = true;
    controls.autoRotateSpeed = 0.25;
    controls.enableDamping = true;

    const textureLoader = new THREE.TextureLoader();

    const earthGeometry = new THREE.SphereGeometry(0.6, 32, 32);
    const earthMaterial = new THREE.MeshPhongMaterial({
        map: textureLoader.load('textures/earthmap1k.jpg'),
        bumpMap: textureLoader.load('textures/earthbump.jpg'),
        bumpScale: 0.3,
        transparent: true,
        opacity: 0.2
    });
    earthMesh = new THREE.Mesh(earthGeometry, earthMaterial);
    earthMesh.rotateZ(-0.3);
    earthMesh.scale.set(0.8, 0.8, 0.8);
    scene.add(earthMesh);

    const cloudGeometry = new THREE.SphereGeometry(0.63, 32, 32);
    const cloudMaterial = new THREE.MeshPhongMaterial({
        map: textureLoader.load('textures/earthCloud.png'),
        transparent: true,
        opacity: 0.2
    });
    cloudMesh = new THREE.Mesh(cloudGeometry, cloudMaterial);
    cloudMesh.scale.set(0.8, 0.8, 0.8);
    scene.add(cloudMesh);

    const ambientLight = new THREE.AmbientLight(0xffffff, 0.01);
    const pointLight = new THREE.PointLight(0xffffff, 0.3);
    pointLight.position.set(5,3,5);
    scene.add(ambientLight);
    scene.add(pointLight);

    const targetScale = new THREE.Vector3(1, 1, 1);
    new TWEEN.Tween(earthMesh.scale)
        .to(targetScale, 1000)
        .delay(500)
        .easing(TWEEN.Easing.Quintic.Out)
        .start()
        .onComplete(() => tweenCompleted = true)

    new TWEEN.Tween(cloudMesh.scale)
        .to(targetScale, 1000)
        .delay(500)
        .easing(TWEEN.Easing.Quintic.Out)
        .start();

    new TWEEN.Tween(earthMesh.material)
        .to({ opacity : 1 }, 1000)
        .delay(500)
        .easing(TWEEN.Easing.Quintic.Out)
        .start();

    new TWEEN.Tween(cloudMesh.material)
        .to({ opacity: 1 }, 1000)
        .delay(500)
        .easing(TWEEN.Easing.Quintic.Out)
        .start();

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