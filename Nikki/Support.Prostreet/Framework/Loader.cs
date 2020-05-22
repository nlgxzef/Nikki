﻿using System.IO;
using Nikki.Core;
using Nikki.Utils;
using Nikki.Reflection.ID;
using Nikki.Support.Prostreet.Class;



namespace Nikki.Support.Prostreet.Framework
{
	internal static class Loader
	{
		#region Private Reading

		private static void ReadMaterial(BinaryReader br, Database.Prostreet db)
		{
			var Class = new Material(br, db);
			db.Materials.Collections.Add(Class);
		}

		private static void ReadTPKBlock(BinaryReader br, byte[] settings, Database.Prostreet db)
		{
			br.BaseStream.Position -= 8;
			var Class = new TPKBlock(db.TPKBlocks.Length, db);
			Class.Disassemble(br);
			Class.SettingData = settings;
			db.TPKBlocks.Collections.Add(Class);
		}

		private static void ReadCarTypeInfos(BinaryReader br, int size, Database.Prostreet db)
		{
			br.BaseStream.Position += 8;
			int len = size / CarTypeInfo.BaseClassSize;

			for (int a1 = 0; a1 < len; ++a1)
			{
				var Class = new CarTypeInfo(br, db);
				db.CarTypeInfos.Collections.Add(Class);
			}
		}

		private static void ReadCarParts(BinaryReader br, int size, Database.Prostreet db) =>
			CarPartManager.Disassemble(br, size, db);

		private static void ReadCollisions(BinaryReader br, int size, Database.Prostreet db)
		{
			var offset = br.BaseStream.Position + size;

			while (br.BaseStream.Position < offset)
			{
				var Class = new Collision(br, db);
				db.Collisions.Collections.Add(Class);
			}
		}

		private static void ReadSunInfos(BinaryReader br, int size, Database.Prostreet db)
		{
			int len = size / SunInfo.BaseClassSize;
			br.BaseStream.Position += 8;

			for (int a1 = 0; a1 < len; ++a1)
			{
				var Class = new SunInfo(br, db);
				db.SunInfos.Collections.Add(Class);
			}
		}

		private static void ReadTracks(BinaryReader br, int size, Database.Prostreet db)
		{
			int len = size / Track.BaseClassSize;

			for (int a1 = 0; a1 < len; ++a1)
			{
				var Class = new Track(br, db);
				db.Tracks.Collections.Add(Class);
			}
		}

		private static void ReadFNGroup(BinaryReader br, Database.Prostreet db)
		{
			br.BaseStream.Position -= 8;
			var Class = new FNGroup(br, db);
			if (Class.Destroy) return;
			db.FNGroups.Collections.Add(Class);
		}

		private static void ReadSTRBlock(BinaryReader br, Database.Prostreet db)
		{
			br.BaseStream.Position -= 8;
			var Class = new STRBlock(br, db);
			db.STRBlocks.Collections.Add(Class);
		}

		#endregion

		public static bool Invoke(Options options, Database.Prostreet db)
		{
			if (!File.Exists(options.File)) return false;
			if (options.Flags.HasFlag(eOptFlags.None)) return false;
			db.Buffer = File.ReadAllBytes(options.File);

			db.Buffer = JDLZ.Decompress(db.Buffer);

			using var ms = new MemoryStream(db.Buffer);
			using var br = new BinaryReader(ms);

			byte[] tempbuf = null;

			while (br.BaseStream.Position < br.BaseStream.Length)
			{
				var ID = br.ReadUInt32();
				var size = br.ReadInt32();

				switch (ID)
				{
					case Global.Materials:
						if (options.Flags.HasFlag(eOptFlags.Materials))
						{
							ReadMaterial(br, db);
							break;
						}
						else goto default;

					case Global.TPKBlocks:
						if (options.Flags.HasFlag(eOptFlags.TPKBlocks))
						{
							ReadTPKBlock(br, tempbuf, db);
							break;
						}
						else goto default;

					case Global.TPKSettings:
						if (options.Flags.HasFlag(eOptFlags.TPKBlocks))
						{
							tempbuf = br.ReadBytes(size);
							break;
						}
						else goto default;

					case Global.CarTypeInfos:
						if (options.Flags.HasFlag(eOptFlags.CarTypeInfos))
						{
							ReadCarTypeInfos(br, size, db);
							break;
						}
						else goto default;

					case Global.Collisions:
						if (options.Flags.HasFlag(eOptFlags.Collisions))
						{
							ReadCollisions(br, size, db);
							break;
						}
						else goto default;

					case Global.SunInfos:
						if (options.Flags.HasFlag(eOptFlags.SunInfos))
						{
							ReadSunInfos(br, size, db);
							break;
						}
						else goto default;

					case Global.Tracks:
						if (options.Flags.HasFlag(eOptFlags.Tracks))
						{
							ReadTracks(br, size, db);
							break;
						}
						else goto default;

					case Global.CarParts:
						if (options.Flags.HasFlag(eOptFlags.DBModelParts))
						{
							ReadCarParts(br, size, db);
							break;
						}
						else goto default;

					case Global.FEngFiles:
					case Global.FNGCompress:
						if (options.Flags.HasFlag(eOptFlags.FNGroups))
						{
							ReadFNGroup(br, db);
							break;
						}
						else goto default;

					case Global.STRBlocks:
						if (options.Flags.HasFlag(eOptFlags.STRBlocks))
						{
							ReadSTRBlock(br, db);
							break;
						}
						else goto default;

					default:
						br.BaseStream.Position += size;
						break;
				}
			}
			return true;
		}
	}
}