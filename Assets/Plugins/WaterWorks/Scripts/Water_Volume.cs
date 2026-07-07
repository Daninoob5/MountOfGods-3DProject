using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Water_Volume : ScriptableRendererFeature
{
    class CustomRenderPass : ScriptableRenderPass
    {
        private Material _material;

        private RTHandle source;
        private RTHandle tempTexture;

        public CustomRenderPass(Material material)
        {
            _material = material;
        }

        public void SetTarget(RTHandle sourceHandle)
        {
            source = sourceHandle;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            RenderTextureDescriptor desc = renderingData.cameraData.cameraTargetDescriptor;
            desc.depthBufferBits = 0;

            RenderingUtils.ReAllocateIfNeeded(
                ref tempTexture,
                desc,
                FilterMode.Bilinear,
                TextureWrapMode.Clamp,
                name: "_TemporaryColourTexture"
            );
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (renderingData.cameraData.camera.cameraType == CameraType.Reflection)
                return;

            CommandBuffer cmd = CommandBufferPool.Get("Water Volume");

            // source -> temp (aplicando material)
            Blitter.BlitCameraTexture(cmd, source, tempTexture, _material, 0);

            // temp -> source
            Blitter.BlitCameraTexture(cmd, tempTexture, source);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            // No liberar aquí si reutilizas el RTHandle
        }

        public void Dispose()
        {
            tempTexture?.Release();
        }
    }

    [System.Serializable]
    public class Settings
    {
        public Material material;
        public RenderPassEvent renderPass = RenderPassEvent.AfterRenderingSkybox;
    }

    public Settings settings = new();

    private CustomRenderPass m_ScriptablePass;

    public override void Create()
    {
        if (settings.material == null)
        {
            settings.material = Resources.Load<Material>("Water_Volume");
        }

        m_ScriptablePass = new CustomRenderPass(settings.material)
        {
            renderPassEvent = settings.renderPass
        };
    }

    public override void AddRenderPasses(
        ScriptableRenderer renderer,
        ref RenderingData renderingData)
    {
        if (settings.material == null)
            return;

        m_ScriptablePass.SetTarget(renderer.cameraColorTargetHandle);

        renderer.EnqueuePass(m_ScriptablePass);
    }

    protected override void Dispose(bool disposing)
    {
        m_ScriptablePass?.Dispose();
    }
}
