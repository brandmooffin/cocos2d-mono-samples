<div align="center">

![Cocos2D-Mono](https://raw.githubusercontent.com/Cocos2D-Mono/cocos2d-mono/master/Logos/logo-full-200.png)

### MonoGame powered built the cocos2d way!

[Check out the docs!](https://cocos2d-mono.dev)
</div>

# Cocos2D-Mono Samples
Here you will find Samples using cocos2d-mono, an easy to use library for simple games using C# and leveraging MonoGame under the hood.

Cocos2D-Mono can be found here: https://github.com/Cocos2D-Mono/cocos2d-mono


These samples are MIT licensed (see [License](#license) below). The Cocos2D-Mono engine is licensed separately, under AGPL-3.0 with a commercial license option — review the [engine license](https://github.com/Cocos2D-Mono/cocos2d-mono/blob/master/LICENSE) when redistributing a build that includes it.

Basic Samples
--------------

Basic samples are very simple and barebone examples for working with cocos2d-mono. These are great samples for a starting point on a fresh project.

Full Samples
------------

Full samples are a little more thoughtful and show more project structure. These are great samples for seeing a more complex approach to working with cocos2d-mono. 


License
-------

This project is open source, freely available, and free of royalties
or encumberance. The software is released under the highly permissive
MIT License.

NuGet
-----
All samples use the NuGet packages for Cocos2D-Mono.

Cocos2D-Mono now ships as a single, multi-targeted package that covers every platform (Windows, Linux, macOS, Android, and iOS) — reference it and the right build is selected automatically from your project's target framework:

  - [Cocos2D-Mono](https://www.nuget.org/packages/Cocos2D-Mono/) — the engine. It takes a dependency on the MonoGame content-pipeline task, but MGCB's MSBuild targets do not flow transitively, so projects that use the content pipeline — these samples included — also reference `MonoGame.Content.Builder.Task` directly.

  - [Cocos2D-Mono.Core](https://www.nuget.org/packages/Cocos2D-Mono.Core/) — the same engine with no MGCB dependency at all, for projects that don't use the content pipeline.

  - [Cocos2D-Mono.Box2D](https://www.nuget.org/packages/Cocos2D-Mono.Box2D/) — the Box2D physics port (also included transitively through the packages above).

Project Templates
-----------------

Project Templates for Visual Studio are available as an extension as well:
[Visual Studio Project Template Extension](https://marketplace.visualstudio.com/items?itemName=Cocos2D-MonoTeamBrokenWallsStudios.cocos2dmonoprojecttemplates)



# Getting Started

Check out the [guides](https://cocos2d-mono.dev/docs/category/getting-started) to find more information on working with Cocos2D-Mono!

# Contributing

Thanks so much for your interest in cocos2d-mono and wanting to contribute to the project! Here's a [guide](https://cocos2d-mono.dev/docs/category/contributing) to help you get started.

Branch from `dev` and target your pull request at `dev` — `main` is release-only. The working conventions shared across every Cocos2D-Mono repository (branching, verification expectations, API stability, release flow) live in [`CONTRIBUTING.md`](https://github.com/Cocos2D-Mono/cocos2d-mono/blob/master/CONTRIBUTING.md) in the engine repository.
