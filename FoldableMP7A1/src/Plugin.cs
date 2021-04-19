using Deli.Setup;

namespace FoldableMP7A1
{
	public class FoldableMP7A1 : DeliBehaviour
	{
		private readonly Hooks _hooks;

		public FoldableMP7A1()
		{
			_hooks = new Hooks();
		}

		private void OnDestroy()
		{
			_hooks?.Dispose();
		}
	}
}