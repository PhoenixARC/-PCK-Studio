using System.Collections.Generic;

namespace stonevox
{
    public class QbManager : Singleton<QbManager>
    {
        private int activemodelindex;

        public bool HasModel { get { return models.Count > 0; } }
        public int ActiveMatrixIndex { get { return ActiveModel.activematrix; } set { SetActiveMatrix(value); } }
        public int ActiveModelIndex { get { return activemodelindex; } set { SetActiveModel(value); } }
        public List<QbModel> models = new List<QbModel>();

        Broadcaster broadcaster;

        public QbModel ActiveModel
        {
            get { return models[activemodelindex]; }
            set { SetActiveModel(value); }
        }

        public QbMatrix ActiveMatrix
        {
            get { return ActiveModel.getactivematrix; }
            set { SetActiveMatrix(value); }
        }

        public QbManager(Broadcaster broadcaster)
            : base()
        {
            this.broadcaster = broadcaster;

            //broadcaster.handlers.Add((message, widge, args) => 
            //{

            //});
        }
        
        void SetActiveModel(int index)
        {
            activemodelindex = index;
            broadcaster.Broadcast(Message.ActiveModelChanged, ActiveModel, ActiveModelIndex);
            broadcaster.Broadcast(Message.ActiveMatrixChanged, ActiveMatrix, ActiveMatrixIndex);
        }

        void SetActiveModel(QbModel model)
        {
            SetActiveModel(models.IndexOf(model));
        }

        void SetActiveMatrix(int index)
        {
           ActiveModel.activematrix = index;
           broadcaster.Broadcast(Message.ActiveMatrixChanged, ActiveMatrix, index);
        }

        void SetActiveMatrix(QbMatrix matrix)
        {
            SetActiveMatrix(ActiveModel.matrices.IndexOf(matrix));
        }

        public void Dispose()
        {
            foreach (var m in models)
                m.Dispose();
        }

        public QbModel AddEmpty()
        {
            QbModel model = QbModel.EmptyModel();
            AddModel(model);
            return model;
        }

        public void AddModel(QbModel model, bool setActive = true)
        {
            models.Add(model);

            if (setActive)
            {
                ActiveModel = model;
                ActiveMatrix.MatchFloorToSize();
            }

            broadcaster.Broadcast(Message.ModelImported, model, model.name);
        }

        public void RemoveModel(QbModel model)
        {
            // stonevox as of now, must have a qbmodel reference...
            // this will be cleanup sometime
            // it's a dirty fix for now :(
            if (ActiveModelIndex == 0 && models.Count == 1)
            {
                if (models.Remove(model))
                {
                    AddEmpty();
                    broadcaster.Broadcast(Message.ModelRemoved, model, model.name);
                    model = null;
                }

                FixActiveMatrix();
                return;
            }
            if (models.Remove(model))
            {
                FixActiveMatrix();
                broadcaster.Broadcast(Message.ModelRemoved, model, model.name);
                model = null;
            }
        }

        void FixActiveMatrix()
        {
            while (activemodelindex >= models.Count)
                activemodelindex--;

            // eh :)
            ActiveModelIndex = activemodelindex;
        }
    }
}