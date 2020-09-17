using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using BattleCity.Core.Enums;
using BattleCity.Core.Models;
using BattleCity.Core.Models.Base;
using BattleCity.Core.Services.Abstractions;
using BattleCity.Core.Services.Implementations;
using Moq;
using Shouldly;
using Xunit;

namespace BattleCity.UnitTests
{
	public class ActionResolverTests
	{
		private readonly Point _respawnA = new Point(40, 40);
		private readonly Point _respawnB = new Point(30, 30);
		private readonly Map _map;

		public ActionResolverTests()
		{
			_map = CreateEmptyMap();
		}

		[Fact]
		public void Should_AddBonusWorksCorrect()
		{
			// Arrange
			var mapPainterMock = new Mock<IMapPainter>();
			var actionResolver = new ActionResolver(mapPainterMock.Object);
			var bonusMock = new Mock<IBonus>();

			// Act
			actionResolver.Initialize(_map);
			actionResolver.Add(bonusMock.Object);

			// Assert
			_map.Bonuses.Count.ShouldBe(1);
			_map.Bonuses.First().ShouldBe(bonusMock.Object);
			mapPainterMock.Verify(_ => _.Draw(bonusMock.Object), Times.Once);
		}

		[Theory]
		[InlineData(Team.A)]
		public void Should_RespawnTankWorksCorrectForAllTeams(Team team)
		{
			// Arrange
			var mapPainterMock = new Mock<IMapPainter>();
			var actionResolver = new ActionResolver(mapPainterMock.Object);

			// Act
			actionResolver.Initialize(_map);
			actionResolver.RespawnTank(team);

			// Assert
			var tankToAssert = team == Team.A ? _map.TankA : _map.TankB;
			var respawnPoint = team == Team.A ? _respawnA : _respawnB;

			tankToAssert.X.ShouldBe(respawnPoint.X);
			tankToAssert.Y.ShouldBe(respawnPoint.Y);
			mapPainterMock.Verify(_ => _.Redraw(tankToAssert), Times.Once);
		}

		private Map CreateEmptyMap() =>
			new Map(
				new List<BrickWall>(),
				new List<ConcreteWall>(),
				new List<River>(),
				new Flag(0, 20),
				new Flag(20, 20),
				_respawnA,
				_respawnB);
	}
}