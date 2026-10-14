# Local ProBuilder 6.1.3 fix

This embedded package preserves upstream assets, source, and license files.

`VertexManipulationTool` now captures its element selection after `BeginEdit`
initialization and holds it until `FinishEdit`. Move/rotate/scale/UV tools apply
absolute deltas relative to the beginning of a gesture. Previously, `RefreshUV`
and `Refresh` emitted `versionChanged`, invalidating `MeshSelection` caches.
The next drag event rebuilt position/UV origins from already modified data and
applied the full gesture delta again, causing accelerating movement.

Editor regression tests cover vertex/edge/face movement through cache rebuilds,
repeated unchanged deltas, returning to the origin, reversing direction, a
second gesture, transformed objects, and repeated scaling.

Validated with Unity 6000.3.25f1 in an isolated project: all four tests pass with
the fix, and all four fail with the upstream `VertexManipulationTool.cs`.
The isolated test project includes this package in `testables` and removes
the upstream `relatedPackages` entry from its package copy because the external
`com.unity.probuilder.tests@6.1.3` package is unavailable. Production package
metadata is unchanged.

When upgrading ProBuilder, retain this fix or verify the upstream package fixes
the same cache lifetime issue before removing the embedded package.
