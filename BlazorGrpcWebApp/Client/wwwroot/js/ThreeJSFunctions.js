import * as THREE from '../js/three.module.js';

let container;
let camera, scene, renderer, cloudParticles = []; 
let ambientLight, directionalLight, flash;
let cloudGeometry, cloudMaterial;
let rainGeometry, rainMaterial, rainCount = 15000, rain, rainDrop;
const vertex = new THREE.Vector3();

function animate() {
    cloudParticles.forEach(p => {
        p.rotation.z -= 0.001;
    });

    let positionAttribute = rain.geometry.getAttribute( 'position' );
    for ( var i = 0; i < positionAttribute.count; i ++ ) {
        vertex.fromBufferAttribute( positionAttribute, i );
        vertex.y -= 0.1 + Math.random() * 0.1;

        if (vertex.y < - 200) {
            vertex.y = 200;
        };

        positionAttribute.setXYZ( i, vertex.x, vertex.y, vertex.z );
        rain.rotation.y += 0.002;
    }

    positionAttribute.needsUpdate = true;

    if(Math.random() > 0.93 || flash.power > 100){
        if(flash.power < 100){
            flash.position.set(Math.random() * 400, 300 + Math.random() * 200, 100);
        };
        flash.power = 50 + Math.random() * 500;
    };

    requestAnimationFrame(animate);
    renderer.render(scene, camera);
};

function loadScene() {

    container = document.getElementById('threejscontainer');
    if (!container) {
        return;
    }

    scene = new THREE.Scene();

    camera = new THREE.PerspectiveCamera(60, container.clientWidth / container.clientHeight, 1, 1000);
    camera.position.z = 1;
    camera.rotation.x = 1.16;
    camera.rotation.y = -0.12;
    camera.rotation.z = 0.27;

    ambientLight = new THREE.AmbientLight(0x555555);
    scene.add(ambientLight);

    directionalLight = new THREE.DirectionalLight(0xffeedd);
    directionalLight.position.set(0, 0, 1);
    scene.add(directionalLight);

    flash = new THREE.PointLight(0x062d89, 30, 500, 1.7);
    flash.position.set(200, 300, 100);
    scene.add(flash);

    rainGeometry = new THREE.BufferGeometry();
    const vertices = [];

    for (let i = 0; i < rainCount; i++) {
        vertices.push( 
		    Math.random() * 400 - 200,
		    Math.random() * 500 - 250,
		    Math.random() * 400 - 200
        );
    }

    rainGeometry.setAttribute( 'position', new THREE.Float32BufferAttribute( vertices, 3 ) );

    rainMaterial = new THREE.PointsMaterial({
        color: 0xaaaaaa, 
        size: 0.1, 
        transparent: true
    });
    rain = new THREE.Points(rainGeometry, rainMaterial);
    scene.add(rain);

    renderer = new THREE.WebGLRenderer({ antialias: true });
    scene.fog = new THREE.FogExp2(0x1c1c2a, 0.002);
    renderer.setPixelRatio(window.devicePixelRatio);
    renderer.setClearColor(scene.fog.color);
    renderer.setSize(container.clientWidth, container.clientHeight);

    let loader = new THREE.TextureLoader();
    loader.load("/img/smoke.png", function(texture){
        cloudGeometry = new THREE.PlaneBufferGeometry(500, 500);
        cloudMaterial = new THREE.MeshLambertMaterial({map: texture, transparent: true});

        for(let i = 0; i < 25; i++) {
            let cloud = new THREE.Mesh(cloudGeometry, cloudMaterial);
            cloud.position.set(Math.random() * 800 - 400, 500, Math.random() * 500 - 450);
            cloud.rotation.x = 1.16;
            cloud.rotation.y = -0.12;
            cloud.rotation.z = Math.random() * 360;
            cloud.material.opacity = 0.6;
            cloudParticles.push(cloud);
            scene.add(cloud);
        }
    });

    animate();

    while (container.lastElementChild) {
        container.removeChild(container.lastElementChild);
    }

    container.appendChild(renderer.domElement);
}

window.ThreeJSFunctions = {
    load: () => { loadScene(); },
};

window.onload = loadScene;
