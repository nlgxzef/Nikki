﻿using System.IO;
using Nikki.Reflection.Abstract;
using Nikki.Reflection.Interface;
using Nikki.Reflection.Attributes;



namespace Nikki.Support.Shared.Parts.BoundParts
{
	/// <summary>
	/// <see cref="CollisionVertex"/> is a unit vertex for <see cref="CollisionCloud"/>.
	/// </summary>
	public class CollisionVertex : ASubPart, ICopyable<CollisionVertex>
	{
		/// <summary>
		/// X coordinate value of this <see cref="CollisionVertex"/>.
		/// </summary>
		[AccessModifiable()]
		public float CoordinateX { get; set; }

		/// <summary>
		/// Y coordinate value of this <see cref="CollisionVertex"/>.
		/// </summary>
		[AccessModifiable()]
		public float CoordinateY { get; set; }

		/// <summary>
		/// Z coordinate value of this <see cref="CollisionVertex"/>.
		/// </summary>
		[AccessModifiable()]
		public float CoordinateZ { get; set; }

		/// <summary>
		/// W coordinate value of this <see cref="CollisionVertex"/>.
		/// </summary>
		[AccessModifiable()]
		public float CoordinateW { get; set; }

		/// <summary>
		/// Creates a plain copy of the objects that contains same values.
		/// </summary>
		/// <returns>Exact plain copy of the object.</returns>
		public CollisionVertex PlainCopy()
		{
			var result = new CollisionVertex()
			{
				CoordinateX = this.CoordinateX,
				CoordinateY = this.CoordinateY,
				CoordinateZ = this.CoordinateZ,
				CoordinateW = this.CoordinateW
			};
			return result;
		}

		/// <summary>
		/// Assembles <see cref="CollisionBound"/> into a byte array.
		/// </summary>
		/// <param name="bw"><see cref="BinaryWriter"/> to write <see cref="CollisionBound"/> with.</param>
		public void Assemble(BinaryWriter bw)
		{
			bw.Write(this.CoordinateX);
			bw.Write(this.CoordinateY);
			bw.Write(this.CoordinateZ);
			bw.Write(this.CoordinateW);
		}

		/// <summary>
		/// Disassembles array into <see cref="CollisionBound"/> properties.
		/// </summary>
		/// <param name="br"><see cref="BinaryReader"/> to read <see cref="CollisionBound"/> with.</param>
		public void Disassemble(BinaryReader br)
		{
			this.CoordinateX = br.ReadInt32();
			this.CoordinateY = br.ReadInt32();
			this.CoordinateZ = br.ReadInt32();
			this.CoordinateW = br.ReadInt32();
		}
	}
}