using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaxyE.Galaxy;
using GalaxyE.Utils;

namespace GalaxyE.Market {
	public class Market {

		public class MarketEntry {

			public TradeGood TradeGood { get; private set; }
			public int Price { get; private set; }
			public int Quantity { get; private set; }
			private Double BasePrice { get; set; }

			public MarketEntry(TradeGood tradeGood, int price, double basePrice, int quantity) {
				this.TradeGood = tradeGood;
				this.Price = price;
				this.Quantity = quantity;
				this.BasePrice = basePrice;
				if (this.Price < 1) this.Price = 1;
			}

			public MarketOperationResult GetBuyQuote(int quantity) {
				int price = 0;
				int basePrice = this.Price;
				int baseQuantity = this.Quantity;

				if (quantity > baseQuantity)
					quantity = baseQuantity;

				if (quantity == 0)
					return new MarketOperationResult(0, 0);

				for (int i = 0; i < quantity; i++) {
					price += basePrice;
					baseQuantity--;

					double bQ = (double)baseQuantity / (double)TradeGood.BaseQuantity;
					double f  = Math.Pow(3, (-(bQ - 1))) + 0.1;
					basePrice = (int)(f * this.BasePrice);
				}

				return new MarketOperationResult(price, quantity);
			}

			public MarketOperationResult GetSellQuote(int quantity) {
				int price = 0;
				int basePrice = this.Price;
				int baseQuantity = this.Quantity;

				if (quantity == 0)
					return new MarketOperationResult(0, 0);

				for (int i = 0; i < quantity; i++) {
					price += basePrice;
					baseQuantity++;

					double bQ = (double)baseQuantity / (double)TradeGood.BaseQuantity;
					double f  = Math.Pow(3, (-(bQ - 1))) + 0.1;
					basePrice = (int)(f * this.BasePrice);

				}

				return new MarketOperationResult(price, quantity);
			}

			public MarketOperationResult Buy(int quantity) {

				int price = 0;

				if (quantity > this.Quantity)
					quantity = this.Quantity;

				if (quantity == 0)
					return new MarketOperationResult(0, 0);

				for (int i = 0; i < quantity; i++) {
					price += this.Price;
					this.Quantity--;

					double bQ = (double)this.Quantity / (double)TradeGood.BaseQuantity;
					double f  = Math.Pow(3, (-(bQ - 1))) + 0.1;
					this.Price = (int)(f * this.BasePrice);
				}

				return new MarketOperationResult(price, quantity);
			}

			public MarketOperationResult Sell(int quantity) {

				int price = 0;

				if (quantity == 0)
					return new MarketOperationResult(0, 0);

				for (int i = 0; i < quantity; i++) {
					price += this.Price;
					this.Quantity++;

					double bQ = (double)this.Quantity / (double)TradeGood.BaseQuantity;
					double f  = Math.Pow(3, (-(bQ - 1))) + 0.1;
					this.Price = (int)(f * this.BasePrice);

				}

				return new MarketOperationResult(price, quantity);
			}

			public override string ToString() {
				return string.Format("{0} {1} {2:0.00} Cr", TradeGood.Name, Quantity, Price / 100.0);
			}

		}

		public class MarketOperationResult {
			public int Money { get; set; }
			public int Quantity { get; set; }

			public MarketOperationResult(int money, int quantity) {
				this.Money = money;
				this.Quantity = quantity;
			}
		}

		public IEnumerable<MarketEntry> GetEntries() {
			foreach (var e in entries.Values)
				yield return e;
		}

		private Dictionary<int, MarketEntry> entries;

		public MarketEntry this[int key] {
			get {
				if (this.entries.ContainsKey(key))
					return this.entries[key];
				else
					return null;
			}
		}

		public bool Contains(int key) {
			return this.entries.ContainsKey(key);
		}

		public Market(SystemBody body, string code) {

			this.entries = new Dictionary<int, MarketEntry>();
			ulong systemId = body.SystemId;
			int seed = Math.Abs(CRC32.Compute(systemId.ToString() + code));

			//int seed = (int)(body.SystemBodyId | (int)((systemId >> 16) & 0xFFFFFFFF));

			Random rand = new Random(seed);

			double timeBase = DateTime.Now.Subtract(new DateTime(2011, 1, 1)).TotalHours;

			double pfluctuation = 1.5 + Math.Sin(timeBase / 2);
			double qfluctuation = 1.5 + Math.Cos(timeBase);
			

			foreach (var tGood in TradeGood.Goods) {

				double quantity;
				double baseQuantity = (double)tGood.BaseQuantity;

				//int overTech = (int)body.StarSystem.TechLevel - tGood.MinTechLevel;

				int overTech = 10;

				bool mainP = overTech > 10;

				if (tGood.MainOrigin.Count > 0)
					mainP &= tGood.MainOrigin.Contains(body.BodyType);

				if (!mainP)
					quantity = rand.Next(0, (int)baseQuantity / 8);
				else {
					quantity = rand.Next((int)baseQuantity / 8, (int)baseQuantity * 2);
					if (tGood.MinTechLevel > 0)
						quantity *= 1 + (overTech * 0.1);
				}

				quantity *= qfluctuation;

				double actBasePrice = pfluctuation * tGood.BasePrice;

				double bQ = quantity / baseQuantity;
				double f  = Math.Pow(3, (-(bQ - 1))) + 0.1;
				var currPrice = f * actBasePrice;

				MarketEntry entry = new MarketEntry(tGood, (int)currPrice, actBasePrice, (int)quantity);

				this.entries.Add(tGood.TradeGoodId, entry);

			}


		}







	}
}
