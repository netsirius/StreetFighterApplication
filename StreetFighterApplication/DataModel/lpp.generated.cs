//---------------------------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated by T4Model template for T4 (https://github.com/linq2db/t4models).
//    Changes to this file may cause incorrect behavior and will be lost if the code is regenerated.
// </auto-generated>
//---------------------------------------------------------------------------------------------------
using System;
using System.Linq;

using LinqToDB;
using LinqToDB.Mapping;

namespace DataModels
{
	/// <summary>
	/// Database       : lpp
	/// Data Source    : tcp://localhost:5432
	/// Server Version : 10.6
	/// </summary>
	public partial class LppDB : LinqToDB.Data.DataConnection
	{
		public ITable<City>   Cities  { get { return this.GetTable<City>(); } }
		public ITable<Game>   Games   { get { return this.GetTable<Game>(); } }
		public ITable<Player> Players { get { return this.GetTable<Player>(); } }

		public LppDB()
		{
			InitDataContext();
		}

		public LppDB(string configuration)
			: base(configuration)
		{
			InitDataContext();
		}

		partial void InitDataContext();
	}

	[Table(Schema="public", Name="cities")]
	public partial class City
	{
		[Column("id"),   PrimaryKey, NotNull] public int    Id   { get; set; } // integer
		[Column("name"),             NotNull] public string Name { get; set; } // character varying(30)
	}

	[Table(Schema="public", Name="games")]
	public partial class Game
	{
		[Column("id"),            PrimaryKey, NotNull] public int    Id          { get; set; } // integer
		[Column("city_id"),                   NotNull] public int    CityId      { get; set; } // integer
		[Column("player_one_id"),             NotNull] public int    PlayerOneId { get; set; } // integer
		[Column("player_two_id"),             NotNull] public int    PlayerTwoId { get; set; } // integer
		[Column("game_result"),               NotNull] public string GameResult  { get; set; } // character varying(10)
	}

	[Table(Schema="public", Name="players")]
	public partial class Player
	{
		[Column("id"),   PrimaryKey, NotNull] public int    Id   { get; set; } // integer
		[Column("name"),             NotNull] public string Name { get; set; } // character varying(30)
	}

	public static partial class TableExtensions
	{
		public static City Find(this ITable<City> table, int Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static Game Find(this ITable<Game> table, int Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}

		public static Player Find(this ITable<Player> table, int Id)
		{
			return table.FirstOrDefault(t =>
				t.Id == Id);
		}
	}
}
