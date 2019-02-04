using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace LuxWater {

	[RequireComponent(typeof(Camera))]
	public class LuxWater_UnderWaterRendering : MonoBehaviour {


		public static LuxWater_UnderWaterRendering instance;

		[Space(8)]
		public Transform Sun;

		[Space(2)]
		[Header("Deep Water Lighting")]
		[Space(4)]
		public bool EnableDeepwaterLighting = false;
		public float DefaultWaterSurfacePosition = 0.0f;
		public float DirectionalLightingFadeRange = 64.0f;
		public float FogLightingFadeRange = 64.0f;

		[Space(2)]
		[Header("Advanced Deferred Fog")]
		[Space(4)]
		public bool EnableAdvancedDeferredFog = false;
		public float FogDepthShift = 1.0f;
		public float FogEdgeBlending = 0.125f;


		[Space(8)]
        [System.NonSerialized]
        public int activeWaterVolume = -1;

[System.NonSerialized]
public List<Camera> activeWaterVolumeCameras = new List<Camera>();

		[System.NonSerialized]
		public float WaterSurfacePos = 0.0f;

		[Space(8)]
		[System.NonSerialized]
		public List<LuxWater_WaterVolume> RegisteredWaterVolumes = new List<LuxWater_WaterVolume>();
		private List<Mesh> WaterMeshes = new List<Mesh>();
		private List<Transform> WaterTransforms = new List<Transform>();
		private List<Material> WaterMaterials = new List<Material>();
private List<bool> WaterIsOnScreen = new List<bool>();

		private RenderTexture UnderWaterMask;

		[Space(2)]
		[Header("Managed transparent Materials")]
		[Space(4)]
		public List<Material> m_aboveWatersurface = new List<Material>();
		public List<Material> m_belowWatersurface = new List<Material>();
		
		[Space(2)]
		[Header("Debug")]
		[Space(4)]
		public bool enableDebug = false;
		[Space(8)]

		private Material mat;
		private Material blurMaterial;
		private Material blitMaterial;
		
private Camera cam;
private bool UnderwaterIsSetUp = false;

		private Transform camTransform;
		private Matrix4x4 frustumCornersArray = Matrix4x4.identity;

		private SphericalHarmonicsL2 ambientProbe;
		private Vector3[] directions = new Vector3[] { new Vector3(0.0f, 1.0f, 0.0f) };
		private Color[] AmbientLightingSamples = new Color[1];


	// Metal Support
	// We have to manually grab the depth texture
		private CommandBuffer cb_DepthGrab;
		private CommandBuffer cb_AfterFinalPass;
		
		//public RenderTexture GrabbedScreen;
		private bool DoUnderWaterRendering = false;
		private Matrix4x4 camProj;
		private Vector3[] frustumCorners = new Vector3[4];

		private int UnderWaterMaskPID;
		private int Lux_FrustumCornersWSPID;
		private int Lux_CameraWSPID;

		private int GerstnerEnabledPID;
		private int LuxWaterMask_GerstnerVertexIntensityPID;
			private int GerstnerVertexIntensityPID;
		private int LuxWaterMask_GAmplitudePID;
			private int GAmplitudePID;
		private int LuxWaterMask_GFinalFrequencyPID;
			private int GFinalFrequencyPID;
		private int LuxWaterMask_GSteepnessPID;
			private int GSteepnessPID;
		private int LuxWaterMask_GFinalSpeedPID;
			private int GFinalSpeedPID;
		private int LuxWaterMask_GDirectionABPID;
			private int GDirectionABPID;
		private int LuxWaterMask_GDirectionCDPID;
			private int GDirectionCDPID;

		private int Lux_UnderWaterAmbientSkyLightPID;

		private int Lux_UnderWaterSunColorPID;
		private int Lux_UnderWaterSunDirPID;

		private int Lux_EdgeLengthPID;
		private int Lux_PhongPID;
		private int Lux_WaterMeshScalePID;

		private int Lux_CausticsEnabledPID;
		private int Lux_CausticModePID;
		private int Lux_UnderWaterFogColorPID;
		private int Lux_UnderWaterFogDensityPID;
		private int Lux_UnderWaterFogAbsorptionCancellationPID;
		private int Lux_UnderWaterAbsorptionHeightPID;
		private int Lux_UnderWaterAbsorptionMaxHeightPID;

		private int Lux_MaxDirLightDepthPID;
		private int Lux_MaxFogLightDepthPID;

		private int Lux_UnderWaterDeferredFogParams;

        private bool islinear = false;

		// Use this for initialization
		void OnEnable () {
			if(instance != null) {
				Destroy(this);
			}
			else {
				instance = this;
			}
			mat = new Material(Shader.Find("Hidden/Lux Water/WaterMask"));
			blurMaterial = new Material(Shader.Find("Hidden/Lux Water/BlurEffectConeTap")); //Ceto/BlurEffectConeTap"));
			blitMaterial = new Material(Shader.Find("Hidden/Lux Water/UnderWaterPost"));
		
if(cam == null) {
	cam = GetComponent<Camera>();
}

		//	Metal support – make sure the camera actually renders a depth texture
			cam.depthTextureMode |= DepthTextureMode.Depth;

			//camTransform = this.transform;
			camTransform = cam.transform;

			UnderWaterMaskPID = Shader.PropertyToID("_UnderWaterMask");
			Lux_FrustumCornersWSPID = Shader.PropertyToID("_Lux_FrustumCornersWS");
			Lux_CameraWSPID = Shader.PropertyToID("_Lux_CameraWS");

			GerstnerEnabledPID = Shader.PropertyToID("_GerstnerEnabled");

			LuxWaterMask_GerstnerVertexIntensityPID = Shader.PropertyToID("_LuxWaterMask_GerstnerVertexIntensity");
				GerstnerVertexIntensityPID = Shader.PropertyToID("_GerstnerVertexIntensity");
			LuxWaterMask_GAmplitudePID = Shader.PropertyToID("_LuxWaterMask_GAmplitude");
				GAmplitudePID = Shader.PropertyToID("_GAmplitude");
			LuxWaterMask_GFinalFrequencyPID = Shader.PropertyToID("_LuxWaterMask_GFinalFrequency");
				GFinalFrequencyPID = Shader.PropertyToID("_GFinalFrequency");
			LuxWaterMask_GSteepnessPID = Shader.PropertyToID("_LuxWaterMask_GSteepness");
				GSteepnessPID = Shader.PropertyToID("_GSteepness");
			LuxWaterMask_GFinalSpeedPID = Shader.PropertyToID("_LuxWaterMask_GFinalSpeed");
				GFinalSpeedPID = Shader.PropertyToID("_GFinalSpeed");
			LuxWaterMask_GDirectionABPID = Shader.PropertyToID("_LuxWaterMask_GDirectionAB");
				GDirectionABPID = Shader.PropertyToID("_GDirectionAB");
			LuxWaterMask_GDirectionCDPID = Shader.PropertyToID("_LuxWaterMask_GDirectionCD");
				GDirectionCDPID = Shader.PropertyToID("_GDirectionCD");

			Lux_UnderWaterAmbientSkyLightPID = Shader.PropertyToID("_Lux_UnderWaterAmbientSkyLight");

			Lux_UnderWaterSunColorPID = Shader.PropertyToID("_Lux_UnderWaterSunColor");
			Lux_UnderWaterSunDirPID = Shader.PropertyToID("_Lux_UnderWaterSunDir");

			Lux_EdgeLengthPID = Shader.PropertyToID("_LuxWater_EdgeLength");
			Lux_PhongPID = Shader.PropertyToID("_LuxWater_Phong");
			Lux_WaterMeshScalePID = Shader.PropertyToID("_LuxWater_MeshScale");

			Lux_MaxDirLightDepthPID = Shader.PropertyToID("_MaxDirLightDepth");
			Lux_MaxFogLightDepthPID = Shader.PropertyToID("_MaxFogLightDepth");

			Lux_CausticsEnabledPID = Shader.PropertyToID("_CausticsEnabled");
			Lux_CausticModePID = Shader.PropertyToID("_CausticMode");
			Lux_UnderWaterFogColorPID = Shader.PropertyToID("_Lux_UnderWaterFogColor");
			Lux_UnderWaterFogDensityPID = Shader.PropertyToID("_Lux_UnderWaterFogDensity");
			Lux_UnderWaterFogAbsorptionCancellationPID = Shader.PropertyToID("_Lux_UnderWaterFogAbsorptionCancellation");
			Lux_UnderWaterAbsorptionHeightPID = Shader.PropertyToID("_Lux_UnderWaterAbsorptionHeight");
		    Lux_UnderWaterAbsorptionMaxHeightPID = Shader.PropertyToID("_Lux_UnderWaterAbsorptionMaxHeight");

Lux_UnderWaterDeferredFogParams = Shader.PropertyToID("_LuxUnderWaterDeferredFogParams");


            islinear = (QualitySettings.desiredColorSpace == ColorSpace.Linear) ? true : false;

        //	Deep Water Lighting and advanced deferred Fog
            SetDeepwaterLighting ();
           	SetDeferredFogParams ();
		}

		void CleanUp () {
			instance = null;
			if(UnderWaterMask != null) {
				UnderWaterMask.Release();
				Destroy(UnderWaterMask);
			}
			if(mat)
				Destroy(mat);
			if(blurMaterial)
				Destroy(blurMaterial);
			if(blitMaterial)
				Destroy(blitMaterial);

			Shader.DisableKeyword("LUXWATER_DEEPWATERLIGHTING");
			Shader.DisableKeyword("LUXWATER_DEFERREDFOG");

		}

		void OnDisable () {
			CleanUp ();
		}

	//	Is also called when the scene gets unloaded.
		void OnDestroy() {
			CleanUp ();
		}

		void OnValidate () {
			SetDeepwaterLighting ();
			SetDeferredFogParams ();
		}

		public void SetDeferredFogParams() {
			if(EnableAdvancedDeferredFog) {
            	Shader.EnableKeyword("LUXWATER_DEFERREDFOG");
				Vector4 fogParams = new Vector4( (DoUnderWaterRendering) ? 1 : 0, FogDepthShift, FogEdgeBlending, 0);
				Shader.SetGlobalVector(Lux_UnderWaterDeferredFogParams, fogParams);
			}
			else {
            	Shader.DisableKeyword("LUXWATER_DEFERREDFOG");
			}
		}

		public void SetDeepwaterLighting () {
			if(EnableDeepwaterLighting) {
				Shader.EnableKeyword("LUXWATER_DEEPWATERLIGHTING");
				if(activeWaterVolume != -1) {
					Shader.SetGlobalFloat("_Lux_UnderWaterWaterSurfacePos", WaterSurfacePos);
				}
				else {
					Shader.SetGlobalFloat("_Lux_UnderWaterWaterSurfacePos", DefaultWaterSurfacePosition);
				}
				Shader.SetGlobalFloat("_Lux_UnderWaterDirLightingDepth", DirectionalLightingFadeRange);
				Shader.SetGlobalFloat("_Lux_UnderWaterFogLightingDepth", FogLightingFadeRange);
			}
			else {
				Shader.DisableKeyword("LUXWATER_DEEPWATERLIGHTING");
			}
		}

		public void RegisterWaterVolume(LuxWater_WaterVolume item, bool visible) {
			RegisteredWaterVolumes.Add(item);
			WaterMeshes.Add(item.WaterVolumeMesh);
			WaterMaterials.Add(item.transform.GetComponent<Renderer>().sharedMaterial);
			WaterTransforms.Add(item.transform);

			WaterIsOnScreen.Add(visible);

		}

		public void DeRegisterWaterVolume(LuxWater_WaterVolume item) {
			int index = RegisteredWaterVolumes.IndexOf(item);

			if (activeWaterVolume == index) {
				activeWaterVolume = -1;
			}

			RegisteredWaterVolumes.RemoveAt(index);
			WaterMeshes.RemoveAt(index);
			WaterMaterials.RemoveAt(index);
			WaterTransforms.RemoveAt(index);

			WaterIsOnScreen.RemoveAt(index);
		}

		public void SetWaterVisible( LuxWater_WaterVolume item) {
			int index = RegisteredWaterVolumes.IndexOf(item);
			WaterIsOnScreen[index] = true;
		}
		public void SetWaterInvisible( LuxWater_WaterVolume item) {
			int index = RegisteredWaterVolumes.IndexOf(item);
			WaterIsOnScreen[index] = false;
		}

		public void EnteredWaterVolume (LuxWater_WaterVolume item, Camera triggerCam) {
			DoUnderWaterRendering = true;
			int index = RegisteredWaterVolumes.IndexOf(item);
			if(index != activeWaterVolume) {
				activeWaterVolume = index;
				
				WaterSurfacePos = WaterTransforms[activeWaterVolume].position.y;

			//	Update Transparents
				for (int i = 0; i < m_aboveWatersurface.Count; i++) {
					m_aboveWatersurface[i].renderQueue = 2997; // 2998
				}
				for (int i = 0; i < m_belowWatersurface.Count; i++) {
					m_belowWatersurface[i].renderQueue = 3001;
				}
			}

			if (!activeWaterVolumeCameras.Contains(triggerCam)) {
				activeWaterVolumeCameras.Add(triggerCam);
			}

		}

		public void LeftWaterVolume (LuxWater_WaterVolume item, Camera triggerCam) {
			DoUnderWaterRendering = false;
			int index = RegisteredWaterVolumes.IndexOf(item);
			if (activeWaterVolume == index) {
				activeWaterVolume = -1;
			//	Update Transparents
				for (int i = 0; i < m_aboveWatersurface.Count; i++) {
					m_aboveWatersurface[i].renderQueue = 3000;
				}
				for (int i = 0; i < m_belowWatersurface.Count; i++) {
					m_belowWatersurface[i].renderQueue = 2997; // 2998
				}
			}

			if (activeWaterVolumeCameras.Contains(triggerCam)) {
				activeWaterVolumeCameras.Remove(triggerCam);
			}

		}


		void OnPreCull () {
			SetDeferredFogParams();
			RenderWaterMask( cam, false );
		}

		[ImageEffectOpaque]
		void OnRenderImage(RenderTexture src, RenderTexture dest) {
			RenderUnderWater( src, dest, cam, false);
		}


// ---------------------------------------------------------
		public void RenderWaterMask(Camera currentCamera, bool SecondaryCameraRendering) {

		//	As we need _Time when drawing the Underwatermask (Gerstner Waves) we have to use a custom _Lux_Time as _Time will not be updated OnPreCull
			Shader.SetGlobalFloat("_Lux_Time", Time.timeSinceLevelLoad);

		//	Check if currentCamera is actually within the watervolume
			if (!activeWaterVolumeCameras.Contains(currentCamera) && !EnableAdvancedDeferredFog) {
				return;
			}
			
		//	Make sure we do not get the scene camera...
			if(currentCamera == null) {
				currentCamera = GetComponent<Camera>();
			}

			currentCamera.depthTextureMode |= DepthTextureMode.Depth;
			camTransform = currentCamera.transform;



			/*
		//	Check if the CommandBuffer already exists
			if (cb_DepthGrab == null) {
				//var commandBuffers = cam.GetCommandBuffers(CameraEvent.BeforeForwardAlpha);
				var commandBuffers = cam.GetCommandBuffers(CameraEvent.AfterSkybox); //AfterImageEffectsOpaque); // this is where deferred fog gets rendered
				for(int i = 0; i < commandBuffers.Length; i++) {
					if(commandBuffers[i].name == "Lux Water Grab Depth") {
						cb_DepthGrab = commandBuffers[i];
						break;
					}
				}
			}
			if (cb_DepthGrab == null) {
				cb_DepthGrab = new CommandBuffer();
				cb_DepthGrab.name = "Lux Water Grab Depth";
				cam.AddCommandBuffer(CameraEvent.AfterSkybox, cb_DepthGrab);
			}
			cb_DepthGrab.Clear();
			int depthGrabID = Shader.PropertyToID("_Lux_GrabbedDepth");
			cb_DepthGrab.GetTemporaryRT(depthGrabID, -1, -1, 0, FilterMode.Point, RenderTextureFormat.DefaultHDR, RenderTextureReadWrite.Linear);
			cb_DepthGrab.Blit(BuiltinRenderTextureType.CameraTarget, depthGrabID);
			*/

		//	Setup UnderWaterMask and SetRenderTarget upfront to prevent spikes.
			if (!UnderWaterMask) {
				UnderWaterMask = new RenderTexture(currentCamera.pixelWidth, currentCamera.pixelHeight, 24, RenderTextureFormat.ARGB32,  RenderTextureReadWrite.Linear);
			}
		//	if SecondaryCameraRendering = true (Splitscreen) do not resize UnderWaterMask
		//	as the secondary camera might have a smaller frame which would be quite expensive...
			else if (UnderWaterMask.width != currentCamera.pixelWidth && !SecondaryCameraRendering) {
				UnderWaterMask = new RenderTexture(currentCamera.pixelWidth, currentCamera.pixelHeight, 24, RenderTextureFormat.ARGB32,  RenderTextureReadWrite.Linear);
			}
			Graphics.SetRenderTarget(UnderWaterMask);
			
		//	Set frustum corners – which are needed to reconstruct the world position in the underwater post shader.
		//	As this spikes as hell when a volume gets active we simply do it all the time.
	        UnityEngine.Profiling.Profiler.BeginSample("Set up Frustum Corners");
		        currentCamera.CalculateFrustumCorners(new Rect(0, 0, 1, 1), currentCamera.farClipPlane, currentCamera.stereoActiveEye, frustumCorners);
		        var bottomLeft = camTransform.TransformVector(frustumCorners[0]);
		        var topLeft = camTransform.TransformVector(frustumCorners[1]);
		        var topRight = camTransform.TransformVector(frustumCorners[2]);
		        var bottomRight = camTransform.TransformVector(frustumCorners[3]);

		        frustumCornersArray.SetRow(0, bottomLeft);
		        frustumCornersArray.SetRow(1, bottomRight);
		        frustumCornersArray.SetRow(2, topLeft);
		        frustumCornersArray.SetRow(3, topRight);

		        Shader.SetGlobalMatrix(Lux_FrustumCornersWSPID, frustumCornersArray);
		        Shader.SetGlobalVector(Lux_CameraWSPID, camTransform.position);
	        UnityEngine.Profiling.Profiler.EndSample();

		
		//	Render UnderWaterMask
			if(activeWaterVolume > -1) {

			//	Tell all shaders that underwater rendering is active (like e.g. fog shaders)
				Shader.EnableKeyword("LUXWATERENABLED");

			//	In case deep water lighting is disabled we pick DirectionalLightingFadeRange from the water material
	            if(!EnableDeepwaterLighting) {
	            	Shader.SetGlobalFloat("_Lux_UnderWaterDirLightingDepth", WaterMaterials[activeWaterVolume].GetFloat(Lux_MaxDirLightDepthPID));
	            	Shader.SetGlobalFloat("_Lux_UnderWaterFogLightingDepth", WaterMaterials[activeWaterVolume].GetFloat(Lux_MaxFogLightDepthPID));
	            }
	            Shader.SetGlobalFloat("_Lux_UnderWaterWaterSurfacePos", WaterSurfacePos);
	        }
	        else {
				Shader.DisableKeyword("LUXWATERENABLED");
			}

			GL.Clear(true, true, Color.black, 1);
			camProj = currentCamera.projectionMatrix;
			GL.LoadProjectionMatrix(camProj);

			for (int i = 0; i < RegisteredWaterVolumes.Count; i++ ) {

				if( !WaterIsOnScreen[i] && i != activeWaterVolume) {
					continue;
				}
				if( !EnableAdvancedDeferredFog && i != activeWaterVolume) {
					continue;
				}

				UnityEngine.Profiling.Profiler.BeginSample("Set up Gerstner");
				//	Gerstner Waves
					if (WaterMaterials[i].GetFloat(GerstnerEnabledPID) == 1.0f) {
						mat.EnableKeyword("GERSTNERENABLED");
						mat.SetVector(LuxWaterMask_GerstnerVertexIntensityPID, WaterMaterials[i].GetVector(GerstnerVertexIntensityPID) );
						mat.SetVector(LuxWaterMask_GAmplitudePID, WaterMaterials[i].GetVector(GAmplitudePID) );
						mat.SetVector(LuxWaterMask_GFinalFrequencyPID, WaterMaterials[i].GetVector(GFinalFrequencyPID) );
						mat.SetVector(LuxWaterMask_GSteepnessPID, WaterMaterials[i].GetVector(GSteepnessPID) );
						mat.SetVector(LuxWaterMask_GFinalSpeedPID, WaterMaterials[i].GetVector(GFinalSpeedPID) );
						mat.SetVector(LuxWaterMask_GDirectionABPID, WaterMaterials[i].GetVector(GDirectionABPID) );
						mat.SetVector(LuxWaterMask_GDirectionCDPID, WaterMaterials[i].GetVector(GDirectionCDPID) );
					}
					else {
						mat.DisableKeyword("GERSTNERENABLED");
					}
				UnityEngine.Profiling.Profiler.EndSample();

			//	Draw UnderWaterMask
				bool useTessellation = WaterMaterials[i].HasProperty(Lux_EdgeLengthPID);
				if (useTessellation) {
					mat.SetFloat(Lux_EdgeLengthPID, WaterMaterials[i].GetFloat(Lux_EdgeLengthPID));
					mat.SetFloat(Lux_PhongPID, WaterMaterials[i].GetFloat(Lux_PhongPID));
					
					var scale = WaterTransforms[i].lossyScale;
					var maxScale = (float)Math.Max(scale.x, Math.Max(scale.y, scale.z));
					mat.SetFloat(Lux_WaterMeshScalePID, 1.0f / maxScale);
				}

				// As this spikes when a volumes gets activated it has been taken out of the if.
				// Graphics.SetRenderTarget(UnderWaterMask); 
				
				if ( i == activeWaterVolume && activeWaterVolumeCameras.Contains(currentCamera) ) {
					mat.SetPass(0);
					Graphics.DrawMeshNow( WaterMeshes[i], WaterTransforms[i].localToWorldMatrix, 0); // submesh 0 = Water box volume
				}
				
			//	Draw water surface only if volume is active or AdvancedDeferredFog is enabled
				if(  i == activeWaterVolume && activeWaterVolumeCameras.Contains(currentCamera) || EnableAdvancedDeferredFog ) {
					if (useTessellation) {
						if ( i == activeWaterVolume) {
							mat.SetPass(3);
						}
						else {
							mat.SetPass(4);
						}
					}
					else {
						if ( i == activeWaterVolume) {
							mat.SetPass(1);
						}
						else {
							mat.SetPass(2);	
						}
					}
					Graphics.DrawMeshNow( WaterMeshes[i], WaterTransforms[i].localToWorldMatrix, 1); // submesh 1 = Water surface from above
				}

			//	We use vface and draw top and from below in a single pass now	
			/*	if (useTessellation) {
					mat.SetPass(4);
				}
				else {
					mat.SetPass(2);
				}
				Graphics.DrawMeshNow( WaterMeshes[activeWaterVolume], WaterTransforms[activeWaterVolume].localToWorldMatrix, 1); // submesh 1 = Water surface from below
			*/
			}

			Shader.SetGlobalTexture(UnderWaterMaskPID, UnderWaterMask);

		//	Set ambient lighting.
	        ambientProbe = RenderSettings.ambientProbe;
	        ambientProbe.Evaluate(directions, AmbientLightingSamples);
            if (islinear)
	            Shader.SetGlobalColor(Lux_UnderWaterAmbientSkyLightPID, (AmbientLightingSamples[0] * RenderSettings.ambientIntensity).linear);
            else
                Shader.SetGlobalColor(Lux_UnderWaterAmbientSkyLightPID, AmbientLightingSamples[0] * RenderSettings.ambientIntensity);
    	
        }

		public void RenderUnderWater(RenderTexture src, RenderTexture dest, Camera currentCamera, bool SecondaryCameraRendering) {

		//	Check if currentCamera is actually within the watervolume
			if (activeWaterVolumeCameras.Contains(currentCamera)) {

				if(DoUnderWaterRendering && activeWaterVolume > -1) {

				//	We currently only support one active water volume. So we setup the materials only once.
					if (!UnderwaterIsSetUp) {

						if(Sun) {
							Vector3 SunDir = -Sun.forward;
			                var SunLight = Sun.GetComponent<Light>();
			                Color SunColor = SunLight.color * SunLight.intensity;
			                if (islinear) {
			                    SunColor = SunColor.linear;
			                }
							Shader.SetGlobalColor(Lux_UnderWaterSunColorPID, (SunColor) * Mathf.Clamp01( Vector3.Dot(SunDir, Vector3.up) )  );
							Shader.SetGlobalVector(Lux_UnderWaterSunDirPID, -SunDir );
						}

						if (WaterMaterials[activeWaterVolume].GetFloat(Lux_CausticsEnabledPID) == 1) {
							blitMaterial.EnableKeyword("GEOM_TYPE_FROND");
							if (WaterMaterials[activeWaterVolume].GetFloat(Lux_CausticModePID) == 1) {
								blitMaterial.EnableKeyword("GEOM_TYPE_LEAF");
							}
							else {
								blitMaterial.DisableKeyword("GEOM_TYPE_LEAF");
							}
						}
						else {
							blitMaterial.DisableKeyword("GEOM_TYPE_FROND");
						}

		                if (islinear)
						    Shader.SetGlobalColor(Lux_UnderWaterFogColorPID, WaterMaterials[activeWaterVolume].GetColor("_Color").linear );
		                else
		                    Shader.SetGlobalColor(Lux_UnderWaterFogColorPID, WaterMaterials[activeWaterVolume].GetColor("_Color"));

		                Shader.SetGlobalFloat(Lux_UnderWaterFogDensityPID, WaterMaterials[activeWaterVolume].GetFloat("_Density") );
						Shader.SetGlobalFloat(Lux_UnderWaterFogAbsorptionCancellationPID, WaterMaterials[activeWaterVolume].GetFloat("_FogAbsorptionCancellation") );

						Shader.SetGlobalFloat(Lux_UnderWaterAbsorptionHeightPID, WaterMaterials[activeWaterVolume].GetFloat("_AbsorptionHeight") );
						Shader.SetGlobalFloat(Lux_UnderWaterAbsorptionMaxHeightPID, WaterMaterials[activeWaterVolume].GetFloat("_AbsorptionMaxHeight") );

						Shader.SetGlobalFloat("_Lux_UnderWaterAbsorptionDepth", WaterMaterials[activeWaterVolume].GetFloat("_AbsorptionDepth") );
						Shader.SetGlobalFloat("_Lux_UnderWaterAbsorptionColorStrength", WaterMaterials[activeWaterVolume].GetFloat("_AbsorptionColorStrength") );
						Shader.SetGlobalFloat("_Lux_UnderWaterAbsorptionStrength", WaterMaterials[activeWaterVolume].GetFloat("_AbsorptionStrength") );

						Shader.SetGlobalFloat("_Lux_UnderWaterUnderwaterScatteringPower", WaterMaterials[activeWaterVolume].GetFloat("_UnderwaterScatteringPower"));
						Shader.SetGlobalFloat("_Lux_UnderWaterUnderwaterScatteringIntensity", WaterMaterials[activeWaterVolume].GetFloat("_UnderwaterScatteringIntensity"));

		// Shader.SetGlobalFloat(Lux_UnderWaterCausticsMaxDepthPID, WaterMaterials[activeWaterVolume].GetFloat("_CausticsMaxDepth"));

						Shader.SetGlobalTexture("_Lux_UnderWaterCaustics", WaterMaterials[activeWaterVolume].GetTexture("_CausticTex") );
		                Shader.SetGlobalFloat("_Lux_UnderWaterCausticsTiling", WaterMaterials[activeWaterVolume].GetFloat("_CausticsTiling"));
		                Shader.SetGlobalFloat("_Lux_UnderWaterCausticsScale", WaterMaterials[activeWaterVolume].GetFloat("_CausticsScale") );
						Shader.SetGlobalFloat("_Lux_UnderWaterCausticsSpeed", WaterMaterials[activeWaterVolume].GetFloat("_CausticsSpeed") );
						Shader.SetGlobalFloat("_Lux_UnderWaterCausticsTiling", WaterMaterials[activeWaterVolume].GetFloat("_CausticsTiling") );
						Shader.SetGlobalFloat("_Lux_UnderWaterCausticsSelfDistortion", WaterMaterials[activeWaterVolume].GetFloat("_CausticsSelfDistortion") );
						Shader.SetGlobalVector("_Lux_UnderWaterFinalBumpSpeed01", WaterMaterials[activeWaterVolume].GetVector("_FinalBumpSpeed01") );

						//blitMaterial.SetFloat("_Lux_UnderWaterWaterSurfacePos", WaterSurfacePos);
						//blitMaterial.SetVector("_Lux_UnderWaterFogDepthAtten", WaterMaterials[activeWaterVolume].GetVector("_DepthAtten"));

						Shader.SetGlobalVector("_Lux_UnderWaterFogDepthAtten", WaterMaterials[activeWaterVolume].GetVector("_DepthAtten"));

					}

				//	Copy Screen into RT and calculate fog and color attenuation
					RenderTexture UnderwaterTex = RenderTexture.GetTemporary(src.width, src.height, 0, RenderTextureFormat.DefaultHDR); //, RenderTextureReadWrite.Linear);
					Graphics.Blit(src, UnderwaterTex, blitMaterial, 0);
					Shader.SetGlobalTexture("_UnderWaterTex", UnderwaterTex);

	            //	Combine source texture and UnderwaterTex based on the Underwatermask in UnderwaterTex.a
	                Graphics.Blit(src, dest, blitMaterial, 1);
					RenderTexture.ReleaseTemporary(UnderwaterTex);
				}

			//	We have to blit in any case - otherwise the screen will be black.
				else {
					Graphics.Blit(src, dest);
				}

			/*	if(currentCamera == cam) {
					UnderwaterIsSetUp = true;
				}
				else {
					UnderwaterIsSetUp = false;
				}
				*/

			}
		//	We have to blit in any case - otherwise the screen will be black.
			else {
				Graphics.Blit(src, dest);
			}
		}


		#if UNITY_EDITOR
			void OnDrawGizmos() {
				if (enableDebug) {
					if(cam == null || UnderWaterMask == null) // || activeWaterVolume == -1)
						return;

			      	int textureDrawWidth = (int)(cam.aspect * 128.0f);
			        GL.PushMatrix();
			        GL.LoadPixelMatrix(0, Screen.width, Screen.height, 0);
			        Graphics.DrawTexture(new Rect(0, 0, textureDrawWidth, 128), UnderWaterMask);
			        GL.PopMatrix();
			    }
			}

            void OnGUI() {
                if(enableDebug) {
                    var Alignement = GUI.skin.label.alignment;
                    GUI.skin.label.alignment = TextAnchor.MiddleLeft;
                    if (activeWaterVolume == -1) {
                    	GUI.Label(new Rect(10, 0, 160, 40), "Active water volume: none\nRegistered volumes: " + RegisteredWaterVolumes.Count);	
                    }
                    else {
                    	GUI.Label(new Rect(10, 0, 300, 40), "Active water volume: " + RegisteredWaterVolumes[activeWaterVolume].transform.gameObject.name + "\nRegistered volumes: " + RegisteredWaterVolumes.Count ); //+ activeWaterVolume.ToString() );
                    }
                    GUI.skin.label.alignment = Alignement;
                }
            }
		#endif

	}
}
