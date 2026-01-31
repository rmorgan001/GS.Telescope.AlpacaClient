// TelescopeViewer - Babylon.js 3D Scene
// Based on PlayGroundTelescopeViewer.js

/**
 * Main BabylonViewer class - manages telescope mount 3D scene
 */
class BabylonViewer {
    constructor(canvasId, skyboxBase64, compassNorthBase64, compassSouthBase64, primaryHex, secondaryHex, nHemiSphere) {
        this.canvas = document.getElementById(canvasId);
        if (!this.canvas) {
            console.error("Canvas element not found:", canvasId);
            return;
        }

        this.primaryColor = primaryHex.toString() || "#FF0000";   // Default primary color
        this.secondaryColor = secondaryHex.toString() || "#00FF00";  // Default secondary color
        this.nHemiSphere = "true";
        this.nHemiSphere = (this.nHemiSphere === nHemiSphere);

        this.engine = null;
        this.scene = null;
        this.camera = null;

        // Telescope hierarchy components
        this.skybox = null;
        this.pierBase = null;
        this.pierPole = null;
        this.mount = null;
        this.otaPivot = null;
        this.otaMesh = null;

        // Store base64 texture data
        this.skyboxBase64 = skyboxBase64;
        this.compassNorthBase64 = compassNorthBase64;
        this.compassSouthBase64 = compassSouthBase64;

        this.isInitialized = false;

        this.initialize();
    }

    /**
     * Initialize Babylon.js engine and scene
     */
    initialize() {
        try {
            // Create Babylon.js engine
            this.engine = new window.BABYLON.Engine(this.canvas, true, {
                preserveDrawingBuffer: true,
                stencil: true,
                antialias: true
            });

            // Create the scene
            this.scene = this.createScene();

            // Start render loop
            this.engine.runRenderLoop(() => {
                if (this.scene) {
                    this.scene.render();
                }
            });

            // Handle window resize
            window.addEventListener("resize", () => {
                this.engine.resize();
            });

            this.isInitialized = true;
            this.hideLoadingMessage();
            console.log("TelescopeViewer initialized successfully");

        } catch (error) {
            console.error("Failed to initialize TelescopeViewer:", error);
            this.showError("Initialization failed: " + error.message);
        }
    }

    /**
     * Create and configure the 3D scene
     */
    createScene() {
        const scene = new window.BABYLON.Scene(this.engine);

        // Set background color (dark)
        scene.clearColor = new window.BABYLON.Color4(0.05, 0.05, 0.05, 1.0);

        // Create ArcRotateCamera
        this.camera = new window.BABYLON.ArcRotateCamera(
            "Camera",
            -Math.PI / 1.2,
            Math.PI / 3,
            40,
            window.BABYLON.Vector3.Zero(),
            scene
        );

        // Attach camera controls to canvas
        this.camera.attachControl(this.canvas, true);

        // Configure camera limits
        this.camera.lowerRadiusLimit = 5;
        this.camera.upperRadiusLimit = 200;
        this.camera.wheelDeltaPercentage = 0.02;

        // Setup scene components
        this.setupLighting(scene);
        this.createSkybox(scene);
        this.createPierBase(scene);
        this.createPierPole(scene);
        this.createMount(scene);
        this.createOtaPivot(scene);

        return scene;
    }

    /**
     * Setup scene lighting
     */
    setupLighting(scene) {
        // Hemispheric light for ambient illumination
        const hemisphericLight = new window.BABYLON.HemisphericLight(
            "hemisphericLight",
            new window.BABYLON.Vector3(0, 1, 0),
            scene
        );

        hemisphericLight.intensity = 0.7;
        hemisphericLight.diffuse = new window.BABYLON.Color3.FromHexString(this.secondaryColor); // all objects color #FF0000
        hemisphericLight.specular = new window.BABYLON.Color3.FromHexString(this.primaryColor); // glare color
        hemisphericLight.groundColor = new window.BABYLON.Color3(0.2, 0.2, 0.2);

        // Directional light for shadows and highlights
        const directionalLight = new window.BABYLON.DirectionalLight(
            "directionalLight",
            new window.BABYLON.Vector3(-1, -2, -1),
            scene
        );
        directionalLight.position = new window.BABYLON.Vector3(10, 20, 10);
        directionalLight.intensity = 0.5;
    }

    /**
     * Create skybox with equirectangular texture
     */
    createSkybox(scene) {
        this.skybox = window.BABYLON.MeshBuilder.CreateBox("SkyBox", {
            height: 400,
            width: 400,
            depth: 400
        }, scene);

        const skyboxMaterial = new window.BABYLON.StandardMaterial("SkyBox", scene);
        skyboxMaterial.backFaceCulling = false;

        // Create data URL from base64
        if (this.skyboxBase64) {
            const dataUrl = "data:image/jpeg;base64," + this.skyboxBase64;
            console.log("Loading skybox from base64 data URL (length:", this.skyboxBase64.length, ")");
            skyboxMaterial.reflectionTexture = new window.BABYLON.Texture(dataUrl, scene, true);
            skyboxMaterial.reflectionTexture.coordinatesMode = window.BABYLON.Texture.FIXED_EQUIRECTANGULAR_MODE;
        } else {
            console.warn("No skybox texture data provided");
        }

        skyboxMaterial.disableLighting = true;
        this.skybox.material = skyboxMaterial;

        console.log("Skybox created");
    }

    /**
     * Create pier base with compass texture
     */
    createPierBase(scene) {
        const compass = new window.BABYLON.StandardMaterial("Compass", scene);

        // Create data URL from base64 (Northern hemisphere by default)
        if (this.compassNorthBase64) {
            const dataUrl = "data:image/png;base64," + this.compassNorthBase64;
            console.log("Loading compass texture from base64 data URL (length:", this.compassNorthBase64.length, ")");
            compass.diffuseTexture = new window.BABYLON.Texture(dataUrl, scene);
        } else {
            console.warn('No compass texture data provided');
        }

        const faceUv = [];
        faceUv[1] = new window.BABYLON.Vector4(0, 0, 0, 0);

        this.pierBase = window.BABYLON.MeshBuilder.CreateCylinder("PierBase", {
            height: 1,
            diameter: 15,
            faceUV: faceUv,
            tessellation: 50
        }, scene);

        this.pierBase.material = compass;
        console.log("Pier base created");
    }

    /**
     * Create pier pole (vertical column)
     */
    createPierPole(scene) {
        this.pierPole = window.BABYLON.MeshBuilder.CreateCylinder("PierPole", {
            height: 7,
            diameter: 0.5
        }, scene);

        this.pierPole.translate(new window.BABYLON.Vector3(0, 3.5, 0), 1, window.BABYLON.Space.WORLD);
        this.pierPole.material = new window.BABYLON.StandardMaterial("PierPole", scene);
        this.pierPole.parent = this.pierBase;

        console.log("Pier pole created");
    }

    /**
     * Create mount (sphere for RA/Alt rotation)
     */
    createMount(scene) {
        this.mount = window.BABYLON.MeshBuilder.CreateSphere("Mount", {
            diameterX: 1.1,
            diameterY: 1.4,
            diameterZ: 2
        }, scene);

        this.mount.translate(new window.BABYLON.Vector3(0, 7, 0.25), 1, window.BABYLON.Space.WORLD);

        // Set initial rotations (default position)
        this.mount.rotation.x = window.BABYLON.Angle.FromDegrees(-30).radians();  // Altitude = 30°
        this.mount.rotation.z = window.BABYLON.Angle.FromDegrees(0).radians();    // RA = 0°

        this.camera.target = this.mount;
        console.log("Mount created");
    }

    /**
     * Create OTA pivot point (for declination rotation)
     */
    createOtaPivot(scene) {
        this.otaPivot = window.BABYLON.MeshBuilder.CreateIcoSphere("OtaPivot", {
            radius: 0.3,
            flat: false
        }, scene);

        this.otaPivot.material = new window.BABYLON.StandardMaterial("OtaPivot", scene);
        this.otaPivot.material.emissiveColor = window.BABYLON.Color4.FromHexString("#f6dea600");
        this.otaPivot.position = new window.BABYLON.Vector3(0, 0, 0.7);
        this.otaPivot.rotation = new window.BABYLON.Vector3(4.7, 1.5, -1.5);

        // Set initial declination rotation
        this.otaPivot.rotation.z = window.BABYLON.Angle.FromDegrees(270).radians();  // Dec = 270°

        this.otaPivot.parent = this.mount;

        // Center camera on mount
        this.camera.target = this.mount;

        console.log('OTA pivot created');
    }

    /**
     * Load telescope model from base64-encoded OBJ data
     */
    async loadTelescopeModel(objDataBase64, mtlDataBase64) {
        try {
            if (!this.isInitialized) {
                throw new Error("Viewer not initialized");
            }

            console.log("Loading telescope model...");

            // Decode base64 to binary
            const objBinary = atob(objDataBase64);
            const objArray = new Uint8Array(objBinary.length);
            for (let i = 0; i < objBinary.length; i++) {
                objArray[i] = objBinary.charCodeAt(i);
            }

            // Create blob and URL for OBJ
            const objBlob = new Blob([objArray], { type: "model/obj" });
            const objUrl = URL.createObjectURL(objBlob);

            let mtlUrl = null;
            if (mtlDataBase64) {
                const mtlBinary = atob(mtlDataBase64);
                const mtlArray = new Uint8Array(mtlBinary.length);
                for (let i = 0; i < mtlBinary.length; i++) {
                    mtlArray[i] = mtlBinary.charCodeAt(i);
                }
                const mtlBlob = new Blob([mtlArray], { type: "text/plain" });
                mtlUrl = URL.createObjectURL(mtlBlob);
            }

            // Remove existing telescope model if any
            if (this.otaMesh) {
                this.otaMesh.dispose();
                this.otaMesh = null;
            }

            // Import the OBJ model
            const result = await window.BABYLON.SceneLoader.ImportMeshAsync(
                "",
                "",
                objUrl,
                this.scene,
                null,
                ".obj"
            );

            console.log(`Loaded ${result.meshes.length} meshes`);

            // Merge all meshes
            const meshesToMerge = result.meshes.filter(m => m.getTotalVertices() > 0);
            if (meshesToMerge.length > 0) {
                this.otaMesh = window.BABYLON.Mesh.MergeMeshes(
                    meshesToMerge,
                    true,
                    true,
                    undefined,
                    false,
                    true
                );

                if (this.otaMesh) {
                    // Apply material
                    this.otaMesh.material = new window.BABYLON.StandardMaterial("OTA", this.scene);

                    // Scale down (telescope models are often large)
                    this.otaMesh.scaling.scaleInPlace(0.01);

                    // Position relative to world origin, then parent to otaPivot
                    this.otaMesh.translate(new window.BABYLON.Vector3(0, 1, -5.3), 1, window.BABYLON.Space.WORLD);
                    this.otaMesh.parent = this.otaPivot;

                    console.log("Telescope model loaded and positioned successfully");
                }
            }

            // Cleanup URLs
            URL.revokeObjectURL(objUrl);
            if (mtlUrl) {
                URL.revokeObjectURL(mtlUrl);
            }

            return 'Model loaded successfully';

        } catch (error) {
            console.error('Error loading telescope model:', error);
            return `Error loading model: ${error.message}`;
        }
    }

    /**
     * Set telescope rotation based on Altitude, Right Ascension, and Declination
     */
    setTelescopeRotation(altitude, rightAscension, declination) {
        if (!this.mount || !this.otaPivot) {
            console.warn('Telescope components not initialized');
            return;
        }

        // Apply rotations (matching PlayGroundTelescopeViewer.js)
        this.mount.rotation.x = window.BABYLON.Angle.FromDegrees(-altitude).radians();
        this.mount.rotation.z = window.BABYLON.Angle.FromDegrees(-rightAscension).radians();
        this.otaPivot.rotation.z = window.BABYLON.Angle.FromDegrees(declination).radians();

        console.log(`Telescope position: Alt=${altitude}°, RA=${rightAscension}°, Dec=${declination}°`);
    }

    /**
     * Set hemisphere (swap compass texture)
     */
    setHemisphere(isNorthern) {
        if (!this.pierBase || !this.pierBase.material) {
            console.warn('Pier base not initialized');
            return;
        }

        // Use the appropriate base64 data for hemisphere
        const base64Data = isNorthern ? this.compassNorthBase64 : this.compassSouthBase64;
        if (base64Data) {
            const dataUrl = 'data:image/png;base64,' + base64Data;
            console.log('Switching to', isNorthern ? 'Northern' : 'Southern', 'hemisphere compass');
            this.pierBase.material.diffuseTexture = new window.BABYLON.Texture(dataUrl, this.scene);
        } else {
            console.warn('Hemisphere texture data not available');
        }

        console.log(`Hemisphere set to: ${isNorthern ? 'Northern' : 'Southern'}`);
    }

    /**
     * Hide loading message
     */
    hideLoadingMessage() {
        const loadingDiv = document.getElementById('loadingMessage');
        if (loadingDiv) {
            loadingDiv.classList.add('hidden');
        }
    }

    /**
     * Show error message
     */
    showError(message) {
        const loadingDiv = document.getElementById('loadingMessage');
        if (loadingDiv) {
            loadingDiv.innerHTML = `<p style="color: #ff4444;">⚠ Error</p><p style="font-size: 14px;">${message}</p>`;
        }
        console.error(message);
    }
}


