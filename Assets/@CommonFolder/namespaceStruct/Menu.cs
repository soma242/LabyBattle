
/*
 detail
1=> gamequit
2=> closer
 */

using MessagePipe;

namespace MenuScene
{
    public static class SkillHolderSize 
    {
        public static float height = 100f;
    }


    public static class DetailNum
    {
        public static int quitNum = 1;

        public static int titleNum = 2;
    }

    public static class MainNum
    {
        public static int statusNum = 1;

        public static int saveNum = 7;
        public static int closeNum = 8;
    }

    public struct MainToStatusMessage { }
    public struct StatusToMainMessage { }
    public struct StatusToSkillMessage { }
    public struct SkillToStatusMessage { }


    public struct MemberHolderClickMessage
    {
        public int id;
        public MemberHolderClickMessage(int id)
        {
            this.id = id; 
        }
    }

    public struct StatusMemberChangeMessage 
    {
        public int id;
        public StatusMemberChangeMessage(int id)
        {
            this.id = id;
        }
    }

    //disposable‚ÍskillTreeHolder‚ÉW–ñ
    public struct ResetSkillFieldMessage { }


    //MainMenu
    public struct MenuIPointerMessage { }




    //SkillFieldControllerŠÖ˜A
    public interface ISelectSkillNameHolder
    {
        MenuSelectHolderSO holder { get; set; }

        void ISetSelect();
        void IResetSelect();
    }



    public class CommonSelectSkillNameHolder: ISelectSkillNameHolder
    {
        public MenuSelectHolderSO holder { get; set; }

        private System.IDisposable disposableOnDestroy;
        
        public CommonSelectSkillNameHolder(MenuSelectHolderSO holder)
        {
            this.holder = holder;
        }

         ~CommonSelectSkillNameHolder()
        {
            disposableOnDestroy?.Dispose();
        }

        public void ISetSelect()
        {
            var bag = DisposableBag.CreateBuilder();
            holder.upSub.Subscribe(holder.skillLayer, get =>
            {
                holder.preSkillPub.Publish(new SelectPreNameHolder());
            }).AddTo(bag);

            holder.downSub.Subscribe(holder.skillLayer, get =>
            {
                holder.nextSkillPub.Publish(new SelectNextNameHolder());
            }).AddTo(bag);

            holder.leftSub.Subscribe(holder.skillLayer, get =>
            {
                holder.layerPub.Publish(new InputLayer(holder.statusLayer));
                holder.statusPub.Publish(new SkillToStatusMessage());
            }).AddTo(bag);

            holder.menuSub.Subscribe(holder.skillLayer, get =>
            {
                holder.layerPub.Publish(new InputLayer(holder.statusLayer));
                holder.statusPub.Publish(new SkillToStatusMessage());
            }).AddTo(bag);

            disposableOnDestroy = bag.Build();
        }

        public void IResetSelect()
        {
            disposableOnDestroy?.Dispose();
        }
    }

    public class LastSelectSkillNameHolder: ISelectSkillNameHolder
    {
        public MenuSelectHolderSO holder { get; set; }

        private System.IDisposable disposableOnDestroy;

        public LastSelectSkillNameHolder(MenuSelectHolderSO holder)
        {
            this.holder = holder;
        }

        ~LastSelectSkillNameHolder()
        {
            disposableOnDestroy?.Dispose();
        }

        public void ISetSelect() 
        {
            var bag = DisposableBag.CreateBuilder();

            holder.upSub.Subscribe(holder.skillLayer, get =>
            {
                holder.preSkillPub.Publish(new SelectPreNameHolder());
            }).AddTo(bag);

            holder.downSub.Subscribe(holder.skillLayer, get =>
            {
                holder.nextTreePub.Publish(new SelectNextTreeHolder());
            }).AddTo(bag);
            
            holder.leftSub.Subscribe(holder.skillLayer, get =>
            {
                holder.layerPub.Publish(new InputLayer(holder.statusLayer));
                holder.statusPub.Publish(new SkillToStatusMessage());
            }).AddTo(bag);

            holder.menuSub.Subscribe(holder.skillLayer, get =>
            {
                holder.layerPub.Publish(new InputLayer(holder.statusLayer));
                holder.statusPub.Publish(new SkillToStatusMessage());
            }).AddTo(bag);

            disposableOnDestroy = bag.Build();
        }
        public void IResetSelect()
        {
            disposableOnDestroy?.Dispose();
        }
    }
    
    public class FirstSelectSkillNameHolder: ISelectSkillNameHolder
    {
        public MenuSelectHolderSO holder { get; set; }

        private System.IDisposable disposableOnDestroy;

        public FirstSelectSkillNameHolder(MenuSelectHolderSO holder)
        {
            this.holder = holder;
        }

        ~FirstSelectSkillNameHolder()
        {
            disposableOnDestroy?.Dispose();
        }

        public void ISetSelect() 
        {
            var bag = DisposableBag.CreateBuilder();

            holder.upSub.Subscribe(holder.skillLayer, get =>
            {
                holder.preTreePub.Publish(new SelectPreTreeHolder());
            }).AddTo(bag);

            holder.downSub.Subscribe(holder.skillLayer, get =>
            {
                holder.nextSkillPub.Publish(new SelectNextNameHolder());
            }).AddTo(bag);

            holder.leftSub.Subscribe(holder.skillLayer, get =>
            {
                holder.layerPub.Publish(new InputLayer(holder.statusLayer));
                holder.statusPub.Publish(new SkillToStatusMessage());
            }).AddTo(bag);

            holder.menuSub.Subscribe(holder.skillLayer, get =>
            {
                holder.layerPub.Publish(new InputLayer(holder.statusLayer));
                holder.statusPub.Publish(new SkillToStatusMessage());
            }).AddTo(bag);

            disposableOnDestroy = bag.Build();
        }
        public void IResetSelect()
        {
            disposableOnDestroy?.Dispose();
        }
    }

    public class OnlySelectSkillNameHolder: ISelectSkillNameHolder
    {
        public MenuSelectHolderSO holder { get; set; }

        private System.IDisposable disposableOnDestroy;

        public OnlySelectSkillNameHolder(MenuSelectHolderSO holder)
        {
            this.holder = holder;
        }

        ~OnlySelectSkillNameHolder()
        {
            disposableOnDestroy?.Dispose();
        }

        public void ISetSelect() 
        {
            var bag = DisposableBag.CreateBuilder();

            holder.upSub.Subscribe(holder.skillLayer, get =>
            {
                holder.preTreePub.Publish(new SelectPreTreeHolder());
            }).AddTo(bag);

            holder.downSub.Subscribe(holder.skillLayer, get =>
            {
                holder.nextTreePub.Publish(new SelectNextTreeHolder());
            }).AddTo(bag);

            holder.leftSub.Subscribe(holder.skillLayer, get =>
            {
                holder.layerPub.Publish(new InputLayer(holder.statusLayer));
                holder.statusPub.Publish(new SkillToStatusMessage());
            }).AddTo(bag);

            holder.menuSub.Subscribe(holder.skillLayer, get =>
            {
                holder.layerPub.Publish(new InputLayer(holder.statusLayer));
                holder.statusPub.Publish(new SkillToStatusMessage());
            }).AddTo(bag);

            disposableOnDestroy = bag.Build();
        }
        public void IResetSelect()
        {
            disposableOnDestroy?.Dispose();
        }

    }

    public struct SelectNextNameHolder { }
    public struct SelectPreNameHolder { }
    public struct SelectNextTreeHolder { }
    public struct SelectPreTreeHolder { }

    public class SkillDescriptionMessage 
    {
        public string desc;
        public SkillDescriptionMessage(string desc)
        {
            this.desc = desc;
        }
    }
}
