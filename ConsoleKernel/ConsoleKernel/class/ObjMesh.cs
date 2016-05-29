using System;
using System.Runtime.InteropServices;
using OpenTK.Math;
using OpenTK.Graphics;

public class ObjMesh : IDisposable
{
    public ObjMesh(string fileName)
    {
		ObjMeshLoader objMeshLoader = new ObjMeshLoader();
		objMeshLoader.Load(this, fileName);
    }

    public ObjVertex[] Vertices
    {
        get { return vertices; }
        set { vertices = value; }
    }
    ObjVertex[] vertices;

    public ObjTriangle[] Triangles
    {
        get { return triangles; }
        set { triangles = value; }
    }
    ObjTriangle[] triangles;

    public ObjQuad[] Quads
    {
        get { return quads; }
        set { quads = value; }
    }
    ObjQuad[] quads;

    int verticesBufferId;
    int trianglesBufferId;
    int quadsBufferId;

    public void Prepare()
    {
        if (verticesBufferId == 0)
        {
            GL.GenBuffers(1, out verticesBufferId);
            GL.BindBuffer(BufferTarget.ArrayBuffer, verticesBufferId);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertices.Length * Marshal.SizeOf(typeof(ObjVertex))), vertices, BufferUsageHint.StaticDraw);
        }

        if (trianglesBufferId == 0)
        {
            GL.GenBuffers(1, out trianglesBufferId);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, trianglesBufferId);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(triangles.Length * Marshal.SizeOf(typeof(ObjTriangle))), triangles, BufferUsageHint.StaticDraw);
        }

        if (quadsBufferId == 0)
        {
            GL.GenBuffers(1, out quadsBufferId);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, quadsBufferId);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(quads.Length * Marshal.SizeOf(typeof(ObjQuad))), quads, BufferUsageHint.StaticDraw);
        }
    }

    public void Render()
    {
        Prepare();

        GL.PushClientAttrib(ClientAttribMask.ClientVertexArrayBit);
        GL.EnableClientState(EnableCap.VertexArray);
        GL.BindBuffer(BufferTarget.ArrayBuffer, verticesBufferId);
        GL.InterleavedArrays(InterleavedArrayFormat.T2fN3fV3f, Marshal.SizeOf(typeof(ObjVertex)), IntPtr.Zero);

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, trianglesBufferId);
        GL.DrawElements(BeginMode.Triangles, triangles.Length * 3, DrawElementsType.UnsignedInt, IntPtr.Zero);

        if (quads.Length > 0)
        {
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, quadsBufferId);
            GL.DrawElements(BeginMode.Quads, quads.Length * 4, DrawElementsType.UnsignedInt, IntPtr.Zero);
        }

        GL.PopClientAttrib();
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ObjVertex
    {
        public Vector2 TexCoord;
        public Vector3 Normal;
        public Vector3 Vertex;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ObjTriangle
    {
        public int Index0;
        public int Index1;
        public int Index2;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ObjQuad
    {
        public int Index0;
        public int Index1;
        public int Index2;
        public int Index3;
    }

	#region IDisposable Support
	private bool disposedValue = false; // 偵測多餘的呼叫

	protected virtual void Dispose(bool disposing)
	{
		if (!disposedValue)
		{
			if (disposing)
			{
				// TODO: 處置 Managed 狀態 (Managed 物件)。
			}


			GL.DeleteBuffers(1, ref verticesBufferId);
			GL.DeleteBuffers(1, ref trianglesBufferId);
			GL.DeleteBuffers(1, ref quadsBufferId);
			

			// TODO: 釋放 Unmanaged 資源 (Unmanaged 物件) 並覆寫下方的完成項。
			// TODO: 將大型欄位設為 null。

			disposedValue = true;
		}
	}

	// TODO: 僅當上方的 Dispose(bool disposing) 具有會釋放 Unmanaged 資源的程式碼時，才覆寫完成項。
	// ~ObjMesh() {
	//   // 請勿變更這個程式碼。請將清除程式碼放入上方的 Dispose(bool disposing) 中。
	//   Dispose(false);
	// }

	// 加入這個程式碼的目的在正確實作可處置的模式。
	public void Dispose()
	{
		// 請勿變更這個程式碼。請將清除程式碼放入上方的 Dispose(bool disposing) 中。
		Dispose(true);
		// TODO: 如果上方的完成項已被覆寫，即取消下行的註解狀態。
		// GC.SuppressFinalize(this);
	}
	#endregion
}
