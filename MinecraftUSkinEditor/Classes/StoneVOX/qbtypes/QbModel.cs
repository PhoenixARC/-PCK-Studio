using System;
using System.Collections.Generic;

namespace stonevox
{
    public class QbModel : IDisposable
    {
        public string name;
        public List<QbMatrix> matrices;

        public uint version;
        public uint colorFormat;
        public uint zAxisOrientation;
        public uint compressed;
        public uint visibilityMaskEncoded;
        public int numMatrices { get { return matrices.Count; } }

        public int activematrix;

        public QbMatrix getactivematrix { get { return matrices[activematrix]; } }

        QbModel()
        {

        }

        public QbModel(string name)
        {
            this.name = name;
        }

        public void setmatrixcount(uint number)
        {
            matrices = new List<QbMatrix>();

            for (int i = 0; i < number; i++)
                matrices.Add(new QbMatrix());
        }

        public void GenerateVertexBuffers()
        {
            matrices.ForEach(t => t.GenerateVertexBuffers());
        }

        public void FillVertexBuffers()
        {
            matrices.ForEach(t => t.FillVertexBuffers());
        }

        public void Render(Shader shader)
        {
            matrices.ForEach(t => t.Render(shader));
        }

        public void Render()
        {
            matrices.ForEach(t => t.Render());
        }

        public void RenderAll()
        {
            matrices.ForEach(t => t.RenderAll());
        }

        public void RenderAll(Shader shader)
        {
            matrices.ForEach(t => t.RenderAll(shader));
        }

        public void UseMatrixColors()
        {
            matrices.ForEach(t => t.UseMatrixColors());
        }

        public static QbModel EmptyModel()
        {
            QbModel model = new QbModel();
            model.version = 257;
            model.zAxisOrientation = 1;
            model.visibilityMaskEncoded = 1;

            model.setmatrixcount(1);

            model.name = "default";
            model.matrices[0].name = "default";
            model.matrices[0].setsize(15, 15, 15, true);

            model.GenerateVertexBuffers();

            return model;
        }

        public void AddMatrix()
        {
            QbMatrix matrix = new QbMatrix();
            matrix.name = "default";
            matrix.setsize(15, 15, 15, true);
            matrix.GenerateVertexBuffers();
            matrices.Add(matrix);
        }

        public void AddMatrix(int index)
        {
            QbMatrix matrix = new QbMatrix();
            matrix.name = "default";
            matrix.setsize(15, 15, 15, true);
            matrix.GenerateVertexBuffers();
            matrices.Insert(index, matrix);
        }

        public void Remove(QbMatrix matrix)
        {
            Remove(matrices.IndexOf(matrix));
        }

        public void Remove(int index)
        {
            matrices.RemoveAt(index);
        }

        public void Sort()
        {
            matrices.Sort((e, x) => { return e.name.CompareTo(x.name); });
        }

        public void Dispose()
        {
            foreach(var m in matrices)
            {
                m.Dispose();
            }
        }
    }
}