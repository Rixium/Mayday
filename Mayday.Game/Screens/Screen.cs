using System.Collections.Generic;
using Mayday.Game.ECS;
using Mayday.Game.UI;
using Microsoft.Xna.Framework;

namespace Mayday.Game.Screens
{
    public abstract class Screen : IScreen
    {
        public bool IsDebug { get; set; }
        public HashSet<IEntity> Entities { get; set; }
        public IScreenManager ScreenManager { get; set; }
        
        public IUserInterface UserInterface { get; set; }
        
        public Color BackgroundColor { get; set; }
        
        public string Name { get; set; }
        
        protected Screen(string name)
        {
            UserInterface = new UserInterface();
            Name = name;
        }
        
        public abstract void Awake();

        public abstract void Begin();
        
        public virtual void Update()
        {
            if(Entities != null)
                foreach (var entity in Entities) 
                    entity.Update();
            
            UserInterface?.Update();
        }

        public virtual void Draw()
        {
            if (Entities != null)
                foreach (var entity in Entities)
                    entity.Draw();

            UserInterface?.Draw();

            if (!IsDebug) return;
        
            if (Entities != null)
                foreach (var entity in Entities)
                    entity.DrawDebug();

            UserInterface?.DrawDebug();
        }

        public abstract void Finish();
        
        public IEntity CreateEntity(string entityName)
        {
            var entity = new Entity
            {
                Name = entityName
            };

            return AddEntity(entity);
        }

        public IEntity AddEntity(IEntity entity)
        {
            if(Entities == null)
                Entities = new HashSet<IEntity>();
            
            entity.Screen = this;
            Entities.Add(entity);
            return entity;
        }
        
    }
}