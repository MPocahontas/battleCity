using System;
using System.Drawing;
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
		
		public Team Team { get; }

		public int BulletSpeed => _isSpeedIncreased
			? Constants.DefaultBulletSpeed * SpeedMultiplier 
			: Constants.DefaultBulletSpeed;

		private int _oldX;
		private int _oldY;
		private bool _isArmored;
		private bool _isSpeedIncreased;

		public Tank(int x, int y, Direction gunDirection, Team team) : base(x, y, Width, Height)
		{
			_oldX = x;
			_oldY = y;
			GunDirection = gunDirection;
			Team = team;
			IsInvulnerable = true;
			IsAlive = true;

			// after 3 second (InvulnerabilityDurationInSeconds) tank should stop being invulnerable
			Task.Delay(TimeSpan.FromSeconds(InvulnerabilityDurationInSeconds))
				.ContinueWith(t => IsInvulnerable = false);
		}

		public void Move(Direction direction)
		{
			// store old tank position in case of rollback need
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

		/// <summary>
		/// rollback state to previous map position
		/// </summary>
		public void RollbackState()
		{
			X = _oldX;
			Y = _oldY;
		}

		public Rectangle GetOldRectangle() 
			=> new Rectangle(_oldX, _oldY, Width, Height);

		/// <summary>
		/// Changes tank state depending on the bonus
		/// </summary>
		/// <param name="bonus"></param>
		public void Apply(IBonus bonus)
		{
			if (bonus is ArmorBonus)
				_isArmored = true;
			else if (bonus is AttackBonus)
			{
				_isSpeedIncreased = true;

				// after 10 seconds (AttackBonusDurationInSeconds) speed should become normal
				Task.Delay(TimeSpan.FromSeconds(AttackBonusDurationInSeconds))
					.ContinueWith(t => _isSpeedIncreased = false);
			}
		}

		/// <summary>
		/// Change tank state after colliding with a bullet
		/// </summary>
		public void Hit()
		{
			// if tank is invulnerable - do nothing
			if (IsInvulnerable)
				return;

			// remove armor if any
			if (_isArmored)
			{
				_isArmored = false;
				return;
			}

			// kill tank in other cases
			IsAlive = false;
		}
	}
}