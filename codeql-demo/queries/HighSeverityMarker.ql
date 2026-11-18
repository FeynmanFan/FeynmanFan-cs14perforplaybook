/**
 * @name Synthetic high-severity demo alert
 * @description Reports a harmless marker to demonstrate CodeQL query and SARIF reporting.
 * @kind problem
 * @problem.severity error
 * @security-severity 8.0
 * @precision high
 * @id local/csharp-demo/high-severity-marker
 * @tags security
 */

import csharp

from StringLiteral marker
where marker.getValue() = "CODEQL_DEMO_SYNTHETIC_HIGH_SEVERITY"
select marker, "Synthetic demo alert only; this marker is not a vulnerability."
