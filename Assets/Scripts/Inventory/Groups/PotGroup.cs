
namespace InventorySystem {

	// also known as the "choom gang"
	public class PotGroup : ItemGroup<CoinItem> {

		public override string ID {
			get { return "pot"; }
		}

		public PotGroup  (int startCount) : base (startCount) {}
	}
}