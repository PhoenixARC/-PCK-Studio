using System.Collections.Generic;

namespace stonevox
{
    public class UndoRedo : Singleton<UndoRedo>
    {
        public LimitedSizeStack<UndoData> undos;
        public LimitedSizeStack<UndoData> redos;

        public UndoRedo(Input input)
            : base()
        {
            undos = new LimitedSizeStack<UndoData>(100);
            redos = new LimitedSizeStack<UndoData>(100);

            input.AddHandler(new InputHandler()
            {
                Keydownhandler = (e) =>
                {
                    var gui =Singleton<GUI>.INSTANCE;


                    if (e.Key ==OpenTK.Input.Key.Z)
                        Undo();
                    if (e.Key == OpenTK.Input.Key.Y)
                        Redo();
                }
            });
        }

        public void Undo()
        {
            UndoData undo;
            var matrices = Singleton<QbManager>.INSTANCE.ActiveModel.matrices;

            // skip all undodata that doesn't have a matrix
            // (ie, matrix was removed or something)
            while (true)
            {
                if (undos.Count == 0) return;
                undo = undos.Pop();

                if (!Singleton<QbManager>.INSTANCE.ActiveModel.matrices.Contains(undo.matrix))
                    continue;

                if (undo.matrix == null)
                    continue;
                else
                    break;
            }

            Singleton<BrushManager>.INSTANCE.brushes[undo.brush].RemoveVolume(undo.volume, undo.matrix, undo.data);
            undo.matrix.Clean();
            redos.Push(undo);
        }

        public void Redo()
        {
            UndoData redo;
            var matrices = Singleton<QbManager>.INSTANCE.ActiveModel.matrices;

            // skip all undodata that doesn't have a matrix
            // (ie, matrix was removed or something)
            while (true)
            {
                if (redos.Count == 0) return;
                redo = redos.Pop();

                if (!Singleton<QbManager>.INSTANCE.ActiveModel.matrices.Contains(redo.matrix))
                    continue;

                if (redo.matrix == null)
                    continue;
                else
                    break;
            }

            Singleton<BrushManager>.INSTANCE.brushes[redo.brush].AddVolume(redo.volume, redo.matrix, ref redo.color, redo.data);
            redo.matrix.Clean();
            undos.Push(redo);
        }

        public void AddUndo(VoxelBrushType type, QbMatrix matrix, VoxelVolume volume, Colort color, Dictionary<double, VoxelUndoData> data)
        {
            redos.Clear();
            undos.Push(new UndoData(type, matrix, volume, color, data));
        }
    }

    public struct UndoData
    {
        public QbMatrix matrix;
        public Dictionary<double, VoxelUndoData> data;
        public VoxelVolume volume;
        public Colort color;
        public VoxelBrushType brush;

        public UndoData(VoxelBrushType type, QbMatrix matrix, VoxelVolume volume, Colort color, Dictionary<double, VoxelUndoData> data)
        {
            this.brush = type;
            this.matrix = matrix;
            this.volume = volume;
            this.color = color;
            this.data = new Dictionary<double, VoxelUndoData>();

            foreach (var c in data)
            {
                this.data.Add(c.Key, c.Value);
            }
        }
    }

    public struct VoxelUndoData
    {
        public int colorindex;
        public bool changed;
        public byte alphamask;

        public VoxelUndoData(bool changed)
        {
            this.colorindex = 0;
            this.changed = changed;
            this.alphamask = 0;
        }

        public VoxelUndoData(int colorindex, byte alphamask)
        {
            this.colorindex = colorindex;
            this.changed = false;
            this.alphamask = alphamask;
        }
    }

    public class LimitedSizeStack<T> : LinkedList<T>
    {
        private readonly int _maxSize;
        public LimitedSizeStack(int maxSize)
        {
            _maxSize = maxSize;
        }

        public void Push(T item)
        {
            this.AddFirst(item);

            if (this.Count > _maxSize)
                this.RemoveLast();
        }

        public T Pop()
        {
            var item = this.First.Value;
            this.RemoveFirst();
            return item;
        }
    }
}
