using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
namespace MagicByte
{
	public class PostProcessingStack
	{
		int fxSourceId = Shader.PropertyToID("_PostFXSource");
		CommandBuffer buffer = new CommandBuffer { name = "Post Processing Workflow" };
		ScriptableRenderContext context;
		public void setRenderContext(ScriptableRenderContext context)
		{ this.context = context; }

		public void postProcessingDrawing(List<Effect> effects, int sourceId,Camera camera)
		{
			startRenderPass(effects, sourceId, camera);
			context.ExecuteCommandBuffer(buffer);
			buffer.Clear();
		}
		public void drawingEffect(RenderTargetIdentifier from, RenderTargetIdentifier to, Material material, int pass)
		{
			buffer.SetGlobalTexture(fxSourceId, from);
			buffer.SetRenderTarget(to, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store);
			buffer.DrawProcedural(Matrix4x4.identity, material, pass, MeshTopology.Triangles, 3);

			buffer.ReleaseTemporaryRT(fxSourceId);
		}
		//Mono Pass
		int effectsID;
		void stackedEffects(List<Effect> effects)
		{
			effectsID = Shader.PropertyToID("_Effect0");
			for (int i = 1; i < effects.Count; i++)
			{
				for(int o = 0;o<effects[i].passes;o++)
				Shader.PropertyToID("_Effect" + o);
			}
		}

		void startRenderPass(List<Effect> effects, int sourceId,Camera camera) 
		{
			stackedEffects(effects);

			int fromId = sourceId, toId = effectsID;
			int i;
			for (i = 0; i < effects.Count; i++)
			{
				buffer.GetTemporaryRT(toId, camera.pixelWidth, camera.pixelHeight, 0, FilterMode.Bilinear, RenderTextureFormat.DefaultHDR);

				effects[i].renderPasses(this, buffer, fromId, toId, camera);

				fromId = effects[i].ToID;
				toId += effects[i].passes;
			}

			drawingEffect(fromId, BuiltinRenderTextureType.CameraTarget, new Material(Shader.Find("Hidden/Standard")), 0);
		}

	}
}