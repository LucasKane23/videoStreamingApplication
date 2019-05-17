DROP FUNCTION f_cities_list(
);

CREATE FUNCTION f_cities_list(
)
RETURNS TABLE (
	"name" TEXT,
	"countrycode" character(3),
	"district"	TEXT,
	"population" integer
) AS
$BODY$

BEGIN

return query SELECT c.name, c.countrycode, c.district, c.population from city c;

END;

$BODY$

LANGUAGE plpgsql;

select * from f_cities_list();