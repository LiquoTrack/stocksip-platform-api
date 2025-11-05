Feature: Mostrar propuesta de valor de StockSip
  Como visitante del sitio web
  Quiero visualizar claramente la solución y beneficios de StockSip
  Para entender de qué forma puede ayudar a mi negocio

  Scenario: Ver propuesta de valor al ingresar al sitio
    Given que el visitante accede al sitio web de negocio
    When se muestra la sección principal de la página de inicio
    Then el visitante entiende la idea principal del uso de la aplicación
