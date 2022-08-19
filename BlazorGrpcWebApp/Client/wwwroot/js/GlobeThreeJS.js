import * as THREE from '../build/three.module.js';
import { FlakesTexture } from '../js/FlakesTexture.js'
import { RGBELoader } from '../js/RGBELoader.js'
import { OrbitControls } from '../js/OrbitControls.js';

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

    renderer = new THREE.WebGLRenderer({ alpha: true, antialias: true });
    renderer.setPixelRatio(window.devicePixelRatio);
    renderer.setSize(container.clientWidth, container.clientHeight);
    renderer.outputEncoding = THREE.sRGBEncoding;
    renderer.toneMapping = THREE.ACESFilmicToneMapping;
    renderer.toneMappingExposure = 1.0;

    camera = new THREE.PerspectiveCamera(50, container.clientWidth / container.clientHeight, 1, 1000);
    camera.position.set(50, 150, 400);
    controls = new OrbitControls(camera, renderer.domElement);
    controls.autoRotate = true;
    controls.autoRotateSpeed = 0.5;
    controls.enableDamping = true;

    let pointLight = new THREE.PointLight(0xffffff, 1);
    pointLight.position.set(600,300,300);
    scene.add(pointLight);

    let envmaploader = new THREE.PMREMGenerator(renderer);

    new RGBELoader().setPath('textures/').load('cayley_interior_4k.hdr', async function(hdrmap) {
        let envmap = envmaploader.fromCubemap(hdrmap)
        let texture = new THREE.CanvasTexture(new FlakesTexture());
        texture.wrapS = THREE.RepeatWrapping;
        texture.wrapT = THREE.RepeatWrapping;
        texture.repeat.x = 10;
        texture.repeat.y = 6;

        let loader = new THREE.TextureLoader();
        let earthTexture =  await loader.load('img/globeHRes.jpg', renderer.render(scene, camera));

        const gMaterial = {
            clearcoat : 1.0,
            clearcoatRoughness: 0.1,
            metalness: 0.9,
            roughness: 0.5,
            color: 0x8418ca,
            reflectivity: 1.0,
            normalMap: texture,
            normalScale: new THREE.Vector2(0.05, 0.05),
            envMap: envmap.texture,
            map: earthTexture,
        }

        let globeGeometry = new THREE.SphereBufferGeometry(100, 64, 64);
        let globeMaterial = new THREE.MeshPhysicalMaterial(gMaterial);
        let globeMesh = new THREE.Mesh(globeGeometry, globeMaterial);
        scene.add(globeMesh);
    })

    animate();

    while (container.lastElementChild) {
        container.removeChild(container.lastElementChild);
    }

    container.appendChild(renderer.domElement);
}


window.GlobeThreeJS = {
    load: () => { loadScene(); },
};

window.onload = loadScene;