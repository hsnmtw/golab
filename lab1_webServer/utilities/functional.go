package utilities

import (
	"maps"
	"math"
	"slices"
)

func head[A any](list []A) A {
	var item A
	if len(list) > 0 {
		item = list[0]
	}
	return item
}

func tail[A any](list []A) []A {
	if len(list) == 0 {
		return list
	}
	return list[1:]
}

/*
*

	[].Select()
*/
func Project[A any, B any](list []A, prdicate func(item A) B) []B {
	if len(list) == 0 {
		return []B{}
	}
	h := prdicate(head(list))
	t := Project(tail(list), prdicate)
	return slices.Concat([]B{h}, t)
}

/*
*
[].Where()
*/
func Filter[A any](list []A, predicate func(item A) bool) []A {
	if len(list) == 0 {
		return list
	}
	h := head(list)
	t := Filter(tail(list), predicate)
	if predicate(h) {
		return slices.Concat([]A{h}, t)
	}
	return t
}

/*
*
HOF
*/
func Reduce[A any, B any](list []A, predicate func(item A, other B) B, accumulator B) B {
	if len(list) == 0 {
		return accumulator
	}
	h := head(list)
	t := Reduce(tail(list), predicate, accumulator)
	return predicate(h, t)
}

func Flatten[A any](list [][]A) []A {
	return Reduce(list, func(l1 []A, l2 []A) []A { return slices.Concat(l1, l2) }, []A{})
}

func SetOf[A comparable](list []A) []A {
	mp := make(map[A]int)
	for _, elm := range list {
		mp[elm] = 0
	}
	return slices.Collect(maps.Keys(mp))
}

func Sum(list []float32) float32 {
	add := func(a float32, b float32) float32 {
		return a + b
	}
	return Reduce(list, add, 0)
}

func Mean(list []float32) float32 {
	n := len(list)
	if n == 0 {
		return 0
	}
	return float32(Sum(list)) / float32(n)
}

func StdDev(list []float32) float64 {
	n := len(list)
	mean := Mean(list)
	sumSquare := Sum(Project(list, func(x float32) float32 { return (x - mean) * (x - mean) }))
	variance := float64(sumSquare) / float64(n)
	return math.Sqrt(variance)
}
