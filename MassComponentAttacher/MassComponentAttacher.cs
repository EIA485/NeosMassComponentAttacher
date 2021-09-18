using System;
using FrooxEngine;
using FrooxEngine.UIX;
namespace MassComponentAttacher
{
	[Category("Add-Ons")]
	class MassComponentAttacher : Component, ICustomInspector
	{
		public readonly SyncRef<Slot> TargetSlot;
		public readonly Sync<bool> Recursive;
		public readonly SyncRef<Component> TemplateComponent;
		public readonly Sync<bool> AttachAsNew;
		[NameOverride("breakExternalReferences/dontRunOnAttachBehavior")]
		public readonly Sync<bool> BreakStuff;
		public void BuildInspectorUI(UIBuilder ui)
		{
			WorkerInspector.BuildInspectorUI(this, ui);
			Button button = ui.Button("Attach Components", click);
		}
		protected override void OnAttach()
        {
			base.OnAttach();
			TargetSlot.Target = this.Slot;
        }
		public void click(IButton button, ButtonEventData eventData)
		{
			if(TemplateComponent.Target==null)
            {
				button.LabelText = "<color=red>component can not be null";
				return;
            }
			Type type = TemplateComponent.Target.GetType();
			if (Recursive)
			{
				int i =0;
				(TargetSlot.Target??this.Slot).ForeachChild((slot) =>
				{
					Attach(type, slot);
					i++;
				});
				button.LabelText = $"attached {i} components";
			}
			else
			{
				for (int i = 0; i < TargetSlot.Target.ChildrenCount; ++i)
				{
					Attach(type, TargetSlot.Target[i]);
				}
				button.LabelText = $"attached {TargetSlot.Target.ChildrenCount} components";
			}

		}
		private void Attach(Type type, Slot slot)
		{
			if (AttachAsNew.Value)
				slot.AttachComponent(type, !BreakStuff.Value, null); 
            else
            {
				typeof(Slot).GetMethod("DuplicateComponent").MakeGenericMethod(new Type[] { type }).Invoke(slot, new object[] { TemplateComponent.Target, BreakStuff.Value });
			}
		}
	}
}
