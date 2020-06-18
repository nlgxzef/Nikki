﻿using System;
using System.IO;
using Nikki.Core;
using Nikki.Utils;
using Nikki.Reflection.Enum;
using Nikki.Support.Carbon.Class;
using CoreExtensions.IO;



namespace Nikki.Support.Carbon.Framework
{
	/// <summary>
	/// A <see cref="Manager{T}"/> for <see cref="SlotType"/> collections.
	/// </summary>
	public class SlotTypeManager : Manager<SlotType>
	{
		private bool _is_read_only = true;

		/// <summary>
		/// Game to which the class belongs to.
		/// </summary>
		public override GameINT GameINT => GameINT.Carbon;

		/// <summary>
		/// Game string to which the class belongs to.
		/// </summary>
		public override string GameSTR => GameINT.Carbon.ToString();

		/// <summary>
		/// Name of this <see cref="SlotTypeManager"/>.
		/// </summary>
		public override string Name => "SlotTypes";

		/// <summary>
		/// True if this <see cref="Manager{T}"/> is read-only; otherwise, false.
		/// </summary>
		public override bool IsReadOnly => this._is_read_only;

		/// <summary>
		/// Indicates required alighment when this <see cref="SlotTypeManager"/> is being serialized.
		/// </summary>
		public override Alignment Alignment { get; }

		/// <summary>
		/// Initializes new instance of <see cref="SlotTypeManager"/>.
		/// </summary>
		/// <param name="db"><see cref="Datamap"/> to which this manager belongs to.</param>
		public SlotTypeManager(Datamap db)
		{
			this.Database = db;
			this.Extender = 0;
			this.Alignment = Alignment.Default;
		}

		/// <summary>
		/// Assembles collection data into byte buffers.
		/// </summary>
		/// <param name="bw"><see cref="BinaryWriter"/> to write data with.</param>
		/// <param name="mark">Watermark to put in the padding blocks.</param>
		internal override void Assemble(BinaryWriter bw, string mark)
		{
			if (this.Count == 0) return;

			bw.GeneratePadding(mark, this.Alignment);

			// Write CarInfo Animation Hookups
			var dif = 4 - this.Count % 4;
			if (dif == 4) dif = 0;

			bw.WriteEnum(eBlockID.CarInfoAnimHookup);
			bw.Write(this.Count + dif);

			// Write Animations
			foreach (var collection in this)
			{

				bw.WriteEnum(collection.PrimaryAnimation);

			}

			bw.WriteBytes(dif);

			// Write CarInfo Animation Hideups
			bw.WriteEnum(eBlockID.CarInfoAnimHideup);
			bw.Write(0x100);
			for (int loop = 0; loop < 0x40; ++loop) bw.Write(0xFFFFFFFF);

			// Precalculate size
			var manager = this.Database.GetManager(typeof(SlotOverrideManager)) as SlotOverrideManager;
			var size = this.Count * SlotType.BaseClassSize;
			size += manager.Count * SlotOverride.BaseClassSize;

			bw.WriteEnum(eBlockID.SlotTypes);
			bw.Write(size);

			// Write SlotTypes
			foreach (var collection in this)
			{

				collection.Assemble(bw);

			}

			// Write CarSlotInfos
			foreach (var collection in manager)
			{

				collection.Assemble(bw);

			}
		}

		/// <summary>
		/// Disassembles data into separate collections in this <see cref="SlotTypeManager"/>.
		/// </summary>
		/// <param name="br"><see cref="BinaryReader"/> to read data with.</param>
		/// <param name="block"><see cref="Block"/> with offsets.</param>
		internal override void Disassemble(BinaryReader br, Block block)
		{
			if (Block.IsNullOrEmpty(block)) return;
			if (block.BlockID != eBlockID.SlotTypes) return;

			this._is_read_only = false;

			for (int loop = 0; loop < block.Offsets.Count; ++loop)
			{

				br.BaseStream.Position = block.Offsets[loop] + 4;
				var size = br.ReadInt32();

				if (size < 0xB98)
				{

					throw new InvalidDataException("SlotTypes block is corrupted or has invalid data");

				}

				var count = 0xB98 / SlotType.BaseClassSize;
				this.Capacity += count;

				for (int i = 0; i < count; ++i)
				{

					var collection = new SlotType(br, this);
					this.Add(collection);

				}

			}

			this._is_read_only = true;
		}

		/// <summary>
		/// Checks whether CollectionName provided allows creation of a new collection.
		/// </summary>
		/// <param name="cname">CollectionName to check.</param>
		internal override void CreationCheck(string cname)
		{
			throw new ArgumentException("CollectionName of SlotTypes cannot be changed");
		}

		/// <summary>
		/// Exports collection with CollectionName specified to a filename provided.
		/// </summary>
		/// <param name="cname">CollectionName of a collection to export.</param>
		/// <param name="bw"><see cref="BinaryWriter"/> to write data with.</param>
		public override void Export(string cname, BinaryWriter bw)
		{

		}

		/// <summary>
		/// Imports collection from file provided and attempts to add it to the end of 
		/// this <see cref="Manager{T}"/> in case it does not exist.
		/// </summary>
		/// <param name="type">Type of serialization of a collection.</param>
		/// <param name="br"><see cref="BinaryReader"/> to read data with.</param>
		public override void Import(eSerializeType type, BinaryReader br)
		{

		}
	}
}
