# Haxl.Net

## Intro

An implementation of [Haxl](http://community.haskell.org/~simonmar/papers/haxl-icfp14.pdf) for the .net platform.

## Motivation

This is useful for combining results of `Task` results either in sequence or concurrently. We take the lead from the Haxl approach and treat the `Applicative` as concurrent 
and the `Monad` as sequencing.