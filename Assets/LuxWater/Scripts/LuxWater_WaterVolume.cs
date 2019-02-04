using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace LuxWater {

	public class LuxWater_WaterVolume : MonoBehaviour {

		public Mesh WaterVolumeMesh;
		
		private LuxWater_UnderWaterRendering waterrendermanager;
		private bool readyToGo = false;

		void OnEnable () {
			if (WaterVolumeMesh == null) {
				Debug.Log("No WaterVolumeMesh assigned.");
				return;
			}
		//	Register with LuxWater_UnderWaterRendering singleton – using invoke just in order to get around script execution order problems
			Invoke("Register", 0.0f);

		//	Make sure wtaer does not cast shadows as otherwise OnBecameVisible is triggered way too early.
			var rend = GetComponent<Renderer>();
			rend.shadowCastingMode = ShadowCastingMode.Off;
		//	Config water material so it uses fixed watersurface position and _Lux_Time	
			var waterMat = rend.sharedMaterial;
			waterMat.EnableKeyword("USINGWATERVOLUME");
			waterMat.SetFloat("_WaterSurfaceYPos", this.transform.position.y);
		}

		void OnDisable () {
			if (waterrendermanager) {
				waterrendermanager.DeRegisterWaterVolume(this);
			}
			readyToGo = false;
			GetComponent<Renderer>().sharedMaterial.DisableKeyword("USINGWATERVOLUME");
		}

		void Register() {
			waterrendermanager = LuxWater_UnderWaterRendering.instance;

		// 	Check if the renderer is visible. Needed in case the script is enabled when the renderer already is on screen as OnBecameVisible will fail...
			var visible = GetComponent<Renderer>().isVisible;
			waterrendermanager.RegisterWaterVolume(this, visible);
			readyToGo = true;
		}

		void OnBecameVisible() {
			if(readyToGo) {
				waterrendermanager.SetWaterVisible(this);
			}
		}
		void OnBecameInvisible() {
			if(readyToGo) {
				waterrendermanager.SetWaterInvisible(this);
			}
		}


	// 	Handle collision between water volume and camera
		private void OnTriggerEnter(Collider other) {
	        var trigger = other.GetComponent<LuxWater_WaterVolumeTrigger>();
	        if (trigger != null && waterrendermanager != null && readyToGo) {
	        	if (trigger.active == true)
	        		waterrendermanager.EnteredWaterVolume(this, other.GetComponent<Camera>() );
	        }
	    }

	    private void OnTriggerStay(Collider other) {
	        var trigger = other.GetComponent<LuxWater_WaterVolumeTrigger>();
	        if (trigger != null && waterrendermanager != null && readyToGo) {
	        	if (trigger.active == true)
	        		waterrendermanager.EnteredWaterVolume(this, other.GetComponent<Camera>() );
	        }
	    }

	    private void OnTriggerExit(Collider other) {
	        var trigger = other.GetComponent<LuxWater_WaterVolumeTrigger>();
	        if (trigger != null && waterrendermanager != null && readyToGo) {
	        	if (trigger.active == true)
	        		waterrendermanager.LeftWaterVolume(this, other.GetComponent<Camera>() );
	        }
	    }

	/*
		void OnWillRenderObject () {
			//Debug.Log("Render " + Shader.GetGlobalVector("unity_SHBr"));

			UnityEngine.Rendering.SphericalHarmonicsL2 sh2 = RenderSettings.ambientProbe;
			//LightProbes.GetInterpolatedProbe(transform.position, null, out sh2);
			Vector3[] directions = new Vector3[] {
	            new Vector3(0.0f, 1.0f, 0.0f),
	            new Vector3(0.0f, -1.0f, 0.0f)
	        };
	        Color[] results = new Color[2];

	        sh2.Evaluate(directions, results);

	        //Debug.Log("Up " + results[0]);

	        Camera.main.depthTextureMode |= DepthTextureMode.Depth;
		}
	*/
		
	}
}
