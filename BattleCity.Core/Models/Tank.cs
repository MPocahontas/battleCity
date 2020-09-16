using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using BattleCity.Core.Enums;
using BattleCity.Core.Models.Base;
using BattleCity.Core.Models.Bonuses;

namespace BattleCity.Core.Models
{
	public class Tank : BaseMapObject, IRedrawable
	{
		private const int SpeedMultiplier = 2;
		private const int AttackBonusDurationInSeconds = 10;
		private const int InvulnerabilityDurationInSeconds = 3;
		public const int Height = 5;
		public const int Width = 5;

		private const int XSpeed = 2;
		private const int YSpeed = 1;

		public bool IsAlive { get; private set; }

		public bool IsInvulnerable { private get; set; }

		public Direction GunDirection { get; private set; }

		public int BulletSpeed => _isSpeedIncreased
			? Constants.DefaultBulletSpeed * SpeedMultiplier 
			: Constants.DefaultBulletSpeed;

		private int _oldX;
		private int _oldY;
		private bool _isArmored;
		private bool _isSpeedIncreased;

		public Tank(int x, int y, Direction gunDirection) : base(x, y, Width, Height)
		{
			_oldX = x;
			_oldY = y;
			GunDirection = gunDirection;
			IsInvulnerable = true;
			Task.Delay(InvulnerabilityDurationInSeconds)
				.ContinueWith(t => IsInvulnerable = false);
		}

		public void Move(Direction direction)
		{
			_oldX = X;
			_oldY = Y;

			GunDirection = direction;

			switch (direction)
			{
				case Direction.Up:
					Y -= YSpeed;
					break;
				case Direction.Right:
					X += XSpeed;
					break;
				case Direction.Down:
					Y += YSpeed;
					break;
				case Direction.Left:
					X -= XSpeed;
					break;
			}
		}

		public void RollbackState()
		{
			X = _oldX;
			Y = _oldY;
		}

		public Rectangle GetOldRectangle() 
			=> new Rectangle(_oldX, _oldY, Width, Height);

		public void Apply(IBonus bonus)
		{
			if (bonus is ArmorBonus)
				_isArmored = true;
			else if (bonus is AttackBonus)
			{
				_isSpeedIncreased = true;
				Task.Delay(TimeSpan.FromSeconds(AttackBonusDurationInSeconds))
					.ContinueWith(t => DecreaseSpeed());
				Task.Run(DecreaseSpeed);
			}
		}

		public void Hit()
		{
			if (IsInvulnerable)
				return;

			if (_isArmored)
			{
				_isArmored = false;
				return;
			}

			IsAlive = false;
		}

		public void DecreaseSpeed()
		{
			_isSpeedIncreased = false;
		}
	}
}