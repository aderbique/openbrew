# Security Policy

## Supported Versions

Security fixes are made against the current `1.0.x` release line.  We may
publish a newer patch release rather than backporting a fix to an older image.

| Version | Supported |
| --- | --- |
| Latest `1.0.x` release | :white_check_mark: |
| Earlier releases | :x: |
| Development builds | Best effort only |

For a deployed instance, compare its image tag or the version shown in Admin
Tools with the [latest OpenBrew release](https://github.com/aderbique/openbrew/releases).

## Reporting a Vulnerability

Please **do not open a public GitHub issue** for a suspected vulnerability.

Send a report to [info@openbrew.net](mailto:info@openbrew.net) with the subject
line `OpenBrew security report`. Include:

- a description of the issue and its potential impact;
- the affected URL, version/image tag, and deployment details when relevant;
- clear reproduction steps or a minimal proof of concept; and
- whether you would like public credit after a fix is released.

Please do not include passwords, access tokens, private recipe data, or other
secrets in the report. If encrypted delivery is needed, say so in the initial
message and we will arrange a secure channel.

We aim to acknowledge reports within **5 business days**, provide a status
update within **10 business days**, and coordinate a fix and disclosure date
with the reporter. Timing can vary with severity and release complexity.

## Scope and Safe Harbor

This policy covers the code in this repository and the official OpenBrew
container image. Third-party services and self-hosted deployments may have
their own configuration and operational risks.

We welcome good-faith research that avoids privacy violations, service
disruption, destructive testing, and social engineering. Do not access or
modify data that is not yours, and stop testing if you encounter it. We will
not pursue action for good-faith, policy-compliant research, but this safe
harbor does not authorize activity that violates applicable law.
