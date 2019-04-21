\version "2.16.0"  % necessary for upgrading to future LilyPond versions.

\header{
	title = "test"
	subtitle = "test"
}

tagline = "pure"

\header{
	piece = "test"
}

\relative c' {
	\time 4/4
	\key c \major
	d8' b4 b8 b4 a4 g8 b16 d8 e2
}
