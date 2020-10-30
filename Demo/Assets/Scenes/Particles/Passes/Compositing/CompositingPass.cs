using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Profiling;
using System.Collections.Generic;
using System.Collections;

class CompositingPass : CustomPass
{
    public Material Mat;
    public string MatPass;
    public int ID;

    private Mesh _Quad;

    protected override void Setup(ScriptableRenderContext renderContext, CommandBuffer cmd)
    {
        _Quad = new Mesh();
        _Quad.SetVertices(new List< Vector3 >{
            new Vector3(-1, -1, 0),
            new Vector3( 1, -1, 0),
            new Vector3(-1,  1, 0),
            new Vector3( 1,  1, 0),
        });
        _Quad.SetTriangles(new List<int>{
            0, 3, 1, 0, 2, 3
        }, 0);
        _Quad.RecalculateBounds();
        _Quad.UploadMeshData(false);
    }

    protected override void Execute(ScriptableRenderContext renderContext, CommandBuffer cmd, HDCamera hdCamera, CullingResults cullingResult)
    {
        int pass = Mat.FindPass(MatPass);
        if (pass == -1)
            return;

        float ForwardDistance = hdCamera.camera.nearClipPlane + 0.0001f;
        var trs = Matrix4x4.TRS(
            hdCamera.camera.transform.position,
            hdCamera.camera.transform.rotation,
            Vector3.one);
    
        renderContext.SetupCameraProperties(hdCamera.camera);
        //cmd.SetRenderTarget( ID );
        cmd.DrawMesh(_Quad, trs, Mat, 0, pass);
    }

    protected override void Cleanup()
    {
        CoreUtils.Destroy(_Quad);
    }
}