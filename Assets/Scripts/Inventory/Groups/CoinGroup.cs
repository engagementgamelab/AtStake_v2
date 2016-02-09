
namespace InventorySystem {

	public class CoinGroup : ItemGroup<CoinItem> {

		public override string ID {
			get { return "coins"; }
		}
	}
}