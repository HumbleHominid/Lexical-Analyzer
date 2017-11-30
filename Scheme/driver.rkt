(module driver racket
	(require "lex.rkt")

	(define run  #t)

	(define get-source-file
		(lambda ()
			(begin
				(display "Give csv file> ")
				(let [(path (~a (read)))]
					(if (file-exists? path)
						path
						(get-source-file)
					)
				)
			)
		)
	)

	(define run-analysis
		(lambda ()
			(if run
			  	(begin
					(analyze "" (get-source-file))
				)
				#f
			)
		)
	)

	(run-analysis)
)
