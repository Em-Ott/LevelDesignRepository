using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.ProBuilder;

namespace UnityEditor.ProBuilder
{
    [EditorTool("ProBuilder Drag Regression Probe")]
    class DragRegressionProbe : ProbuilderMoveTool
    {
        public void Begin()
        {
            typeof(VertexManipulationTool).GetProperty("currentEvent",
                BindingFlags.Instance | BindingFlags.NonPublic).SetValue(this, new Event());
            m_HandlePosition = Vector3.zero;
            m_HandleRotation = Quaternion.identity;
            typeof(VertexManipulationTool).GetField("m_HandleRotationOrigin",
                BindingFlags.Instance | BindingFlags.NonPublic).SetValue(this, Quaternion.identity);
            BeginEdit("Drag regression");
        }

        public void Translate(Vector3 delta)
        {
            typeof(ProbuilderMoveTool).GetMethod("ApplyTranslation",
                BindingFlags.Instance | BindingFlags.NonPublic).Invoke(this, new object[] { delta });
        }

        public void TransformSelection(Matrix4x4 delta) => Apply(delta);
        public void Finish() => FinishEdit();
    }

    public class VertexDragRegressionTests
    {
        ProBuilderMesh m_Mesh;
        DragRegressionProbe m_Tool;
        Object[] m_PreviousSelection;
        SelectMode m_PreviousSelectMode;
        bool m_PreviousGridSnap;
        PivotMode m_PreviousPivotMode;
        PivotRotation m_PreviousPivotRotation;

        [SetUp]
        public void SetUp()
        {
            m_PreviousSelection = Selection.objects;
            m_PreviousSelectMode = ProBuilderEditor.selectMode;
            m_PreviousGridSnap = EditorSnapSettings.gridSnapEnabled;
            m_PreviousPivotMode = Tools.pivotMode;
            m_PreviousPivotRotation = Tools.pivotRotation;
            EditorSnapSettings.gridSnapEnabled = false;
            Tools.pivotMode = PivotMode.Center;
            Tools.pivotRotation = PivotRotation.Global;
            m_Mesh = ShapeGenerator.CreateShape(ShapeType.Cube);
            // Also cover translation through rotated, non-uniformly scaled object space.
            m_Mesh.transform.SetPositionAndRotation(new Vector3(7, 2, -3), Quaternion.Euler(10, 35, 20));
            m_Mesh.transform.localScale = new Vector3(2, 3, 0.5f);
            Selection.activeGameObject = m_Mesh.gameObject;
            MeshSelection.OnObjectSelectionChanged();
            m_Tool = ScriptableObject.CreateInstance<DragRegressionProbe>();
            ToolManager.SetActiveTool(m_Tool);
        }

        [TearDown]
        public void TearDown()
        {
            m_Tool.Finish();
            ToolManager.RestorePreviousTool();
            Selection.objects = new Object[0];
            Object.DestroyImmediate(m_Tool);
            Object.DestroyImmediate(m_Mesh.gameObject);
            Selection.objects = m_PreviousSelection;
            ProBuilderEditor.selectMode = m_PreviousSelectMode;
            EditorSnapSettings.gridSnapEnabled = m_PreviousGridSnap;
            Tools.pivotMode = m_PreviousPivotMode;
            Tools.pivotRotation = m_PreviousPivotRotation;
        }

        void Select(SelectMode mode)
        {
            ProBuilderEditor.selectMode = mode;
            if (mode == SelectMode.Vertex)
                m_Mesh.SetSelectedVertices(new[] { 0 });
            else if (mode == SelectMode.Edge)
                m_Mesh.SetSelectedEdges(new[] { m_Mesh.faces[0].edges[0] });
            else
                m_Mesh.SetSelectedFaces(new[] { m_Mesh.faces[0] });
        }

        [TestCase(SelectMode.Vertex)]
        [TestCase(SelectMode.Edge)]
        [TestCase(SelectMode.Face)]
        public void DragUsesGestureOriginDespiteMeshRefresh(SelectMode mode)
        {
            Select(mode);
            m_Tool.Begin();
            var indices = m_Tool.GetElementSelection(m_Mesh, PivotPoint.Center)
                .elementGroups.SelectMany(x => x.indices).Distinct().ToArray();
            var original = m_Mesh.positions.Select(m_Mesh.transform.TransformPoint).ToArray();
            foreach (var distance in new[] { 1f, 2f, 3f, 3f, 0f, -1f })
            {
                m_Tool.Translate(Vector3.right * distance);
                // Rendering/selection consumers may rebuild the shared cache between events.
                var rebuilt = MeshSelection.elementSelection.ToArray();
                Assert.That(rebuilt.Length, Is.EqualTo(1));
                for (var i = 0; i < original.Length; i++)
                {
                    var expected = original[i] + (indices.Contains(i) ? Vector3.right * distance : Vector3.zero);
                    Assert.That(Vector3.Distance(m_Mesh.transform.TransformPoint(m_Mesh.positions[i]), expected),
                        Is.LessThan(0.0001f), "vertex " + i + ", handle distance " + distance);
                }
            }
            m_Tool.Finish();
            var secondOrigin = m_Mesh.transform.TransformPoint(m_Mesh.positions[0]);
            m_Tool.Begin();
            m_Tool.Translate(Vector3.right * 2);
            Assert.That(Vector3.Distance(m_Mesh.transform.TransformPoint(m_Mesh.positions[0]),
                secondOrigin + Vector3.right * 2), Is.LessThan(0.0001f));
        }

        [Test]
        public void RepeatedScaleDoesNotCompound()
        {
            Select(SelectMode.Face);
            m_Tool.Begin();
            m_Tool.TransformSelection(Matrix4x4.Scale(Vector3.one * 2));
            var once = m_Mesh.positions.ToArray();
            var rebuilt = MeshSelection.elementSelection.ToArray();
            m_Tool.TransformSelection(Matrix4x4.Scale(Vector3.one * 2));
            for (var i = 0; i < once.Length; i++)
                Assert.That(Vector3.Distance(once[i], m_Mesh.positions[i]), Is.LessThan(0.0001f));
        }
    }
}
