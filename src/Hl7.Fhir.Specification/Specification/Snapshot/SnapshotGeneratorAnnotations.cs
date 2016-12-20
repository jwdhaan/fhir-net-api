﻿/* 
 * Copyright (c) 2016, Furore (info@furore.com) and contributors
 * See the file CONTRIBUTORS for details.
 * 
 * This file is licensed under the BSD 3-Clause license
 * available at https://raw.githubusercontent.com/ewoutkramer/fhir-net-api/master/LICENSE
 */

using Hl7.Fhir.Model;
using Hl7.Fhir.Support;
using System;
using System.Collections.Generic;

namespace Hl7.Fhir.Specification.Snapshot
{
    // Custom annotations for internal use by the SnapshotGenerator

    public static class SnapshotGeneratorAnnotations
    {
        #region Annotation: Created By Snapshot Generator

        /// <summary>Annotation to mark a generated element, so we can prevent duplicate re-generation.</summary>
        sealed class CreatedBySnapshotGeneratorAnnotation
        {
            public DateTime Created { get; }
            public CreatedBySnapshotGeneratorAnnotation() { Created = DateTime.Now; }
        }

        /// <summary>Marks the specified element as generated by the <see cref="SnapshotGenerator"/>.</summary>
        internal static void SetCreatedBySnapshotGenerator(this Element elem) { elem?.AddAnnotation(new CreatedBySnapshotGeneratorAnnotation()); }

        /// <summary>Determines if the specified element was created by the <see cref="SnapshotGenerator"/>.</summary>
        /// <param name="elem">A FHIR <see cref="Element"/>.</param>
        /// <returns><c>true</c> if the element was created by the <see cref="SnapshotGenerator"/>, or <c>false</c> otherwise.</returns>
        public static bool IsCreatedBySnapshotGenerator(this Element elem) => elem != null && elem.HasAnnotation<CreatedBySnapshotGeneratorAnnotation>();

        #endregion

        #region Annotation: Differential Constraint

        /// <summary>
        /// Indicates elements and properties in the <see cref="StructureDefinition.SnapshotComponent"/>
        /// that are constrained by the <see cref="StructureDefinition.DifferentialComponent"/>.
        /// </summary>
        sealed class ConstrainedByDifferentialAnnotation
        {
            //
        }

        /// <summary>Annotate the specified snapshot element to indicate that it is constrained by the differential.</summary>
        internal static void SetConstrainedByDifferential(this Base element)
        {
            element?.AddAnnotation(new ConstrainedByDifferentialAnnotation());
        }

        /// <summary>Remove any existing differential constraint annotation from the specified snapshot element.</summary>
        internal static void ClearConstrainedByDifferential(this Base element)
        {
            element?.RemoveAnnotations<ConstrainedByDifferentialAnnotation>();
        }

        /// <summary>Recursively remove any existing differential constraint annotations from the specified snapshot element and all it's children.</summary>
        internal static void ClearAllConstrainedByDifferential(this Base element)
        {
            if (element == null) { throw Error.ArgumentNull(nameof(element)); }
            element.ClearConstrainedByDifferential();
            foreach (var child in element.Children)
            {
                child.ClearAllConstrainedByDifferential();
            }
        }

        /// <summary>Recursively remove any existing differential constraint annotations from the specified snapshot elements and all their children.</summary>
        internal static void ClearAllConstrainedByDifferential<T>(this List<T> elements) where T : Base
        {
            if (elements == null) { throw Error.ArgumentNull(nameof(elements)); }
            foreach (var elem in elements)
            {
                elem.ClearAllConstrainedByDifferential();
            }
        }

        public static bool IsConstrainedByDifferential(this Element elem) => elem != null && elem.HasAnnotation<ConstrainedByDifferentialAnnotation>();

        #endregion


        #region Annotation: Snapshot ElementDefinition

        /// <summary>For annotating a differential element definition with a reference to the associated generated snapshot element definition.</summary>
        sealed class SnapshotElementDefinitionAnnotation
        {
            public ElementDefinition SnapshotElement { get; }
            public SnapshotElementDefinitionAnnotation(ElementDefinition snapshotElement)
            {
                if (snapshotElement == null) { throw Error.ArgumentNull(nameof(snapshotElement)); }
                SnapshotElement = snapshotElement;
            }
        }

        /// <summary>
        /// Annotate the root <see cref="ElementDefinition"/> instance in the <see cref="StructureDefinition.Differential"/> component
        /// with a reference to the generated snapshot root <see cref="ElementDefinition"/> instance.
        /// </summary>
        internal static void SetSnapshotRootElementAnnotation(this StructureDefinition sd, ElementDefinition rootElemDef)
        {
            sd?.Differential?.Element[0]?.SetSnapshotElementAnnotation(rootElemDef);
        }

        internal static void SetSnapshotElementAnnotation(this ElementDefinition diffElemDef, ElementDefinition snapElemDef)
        {
            diffElemDef?.AddAnnotation(new SnapshotElementDefinitionAnnotation(snapElemDef));
        }

        /// <summary>
        /// Retrieve a reference to a previously generated snapshot root <see cref="ElementDefinition"/> instance
        /// from the root <see cref="ElementDefinition"/> instance in the <see cref="StructureDefinition.Differential"/> component.
        /// </summary>
        internal static ElementDefinition GetSnapshotRootElementAnnotation(this StructureDefinition sd) => sd?.Differential?.Element[0]?.GetSnapshotElementAnnotation();

        internal static ElementDefinition GetSnapshotElementAnnotation(this ElementDefinition ed) => ed?.Annotation<SnapshotElementDefinitionAnnotation>()?.SnapshotElement;

        /// <summary>Remove all <see cref="SnapshotElementDefinitionAnnotation"/> instances from the specified <see cref="ElementDefinition"/>.</summary>
        internal static void ClearSnapshotElementAnnotations(this ElementDefinition ed) { ed?.RemoveAnnotations<SnapshotElementDefinitionAnnotation>(); }

        #endregion

    }
}
