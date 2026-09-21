import { useEffect, useState } from 'react'
import './App.css'

const emptyOrder = {
  senderCity: '',
  senderAddress: '',
  recipientCity: '',
  recipientAddress: '',
  weight: '',
  pickupDate: '',
}

const apiUrl = 'http://localhost:5080/api/orders'

function App() {
  const [orders, setOrders] = useState([])
  const [selectedOrder, setSelectedOrder] = useState(null)
  const [form, setForm] = useState(emptyOrder)
  const [error, setError] = useState('')
  const [isSaving, setIsSaving] = useState(false)

  useEffect(() => {
    fetch(apiUrl)
      .then((response) => response.json())
      .then(setOrders)
      .catch(() =>
        setError('Не удалось загрузить отправления. Проверьте, запущен ли backend.')
      )
  }, [])

  const updateField = (event) => {
    setForm({
      ...form,
      [event.target.name]: event.target.value,
    })
  }

  const createOrder = async (event) => {
    event.preventDefault()
    setError('')
    setIsSaving(true)

    try {
      const response = await fetch(apiUrl, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          ...form,
          weight: Number(form.weight),
        }),
      })

      if (!response.ok) {
        throw new Error()
      }

      const created = await response.json()
      setOrders([created, ...orders])
      setSelectedOrder(created)
      setForm(emptyOrder)
    } catch {
      setError('Не удалось создать отправление. Проверьте соединение с сервером.')
    } finally {
      setIsSaving(false)
    }
  }

  return (
    <div className="app-shell">
      <main className="workspace">
        <section className="intro">
          <div>
            <h2>
              Оформите отправление
              <br />
              <span className="intro-note">
                Заполните данные отправителя и получателя.
                <br />
                Номер отправления присвоится автоматически.
              </span>
            </h2>
          </div>
        </section>

        <div className="dashboard-grid">
          <form className="panel order-form" onSubmit={createOrder}>
            <div className="panel-heading">
              <div>
                <h3>Новое отправление</h3>
              </div>
              <span className="required-note">* обязательные поля</span>
            </div>

            <div className="form-section">
              <div className="section-label">ОТКУДА</div>
              <div className="field-grid">
                <label>
                  <span className="field-label">
                    Город <span className="required-mark">*</span>
                  </span>
                  <input
                    name="senderCity"
                    value={form.senderCity}
                    onChange={updateField}
                    placeholder="Например, Москва"
                    required
                  />
                </label>

                <label>
                  <span className="field-label">
                    Адрес <span className="required-mark">*</span>
                  </span>
                  <input
                    name="senderAddress"
                    value={form.senderAddress}
                    onChange={updateField}
                    placeholder="Улица, дом, квартира"
                    required
                  />
                </label>
              </div>
            </div>

            <div className="form-section">
              <div className="section-label">КУДА</div>
              <div className="field-grid">
                <label>
                  <span className="field-label">
                    Город <span className="required-mark">*</span>
                  </span>
                  <input
                    name="recipientCity"
                    value={form.recipientCity}
                    onChange={updateField}
                    placeholder="Например, Казань"
                    required
                  />
                </label>

                <label>
                  <span className="field-label">
                    Адрес <span className="required-mark">*</span>
                  </span>
                  <input
                    name="recipientAddress"
                    value={form.recipientAddress}
                    onChange={updateField}
                    placeholder="Улица, дом, квартира"
                    required
                  />
                </label>
              </div>
            </div>

            <div className="form-section compact-fields">
              <label>
                <span className="field-label">
                  Вес груза, кг <span className="required-mark">*</span>
                </span>
                <input
                  type="number"
                  name="weight"
                  value={form.weight}
                  onChange={updateField}
                  min="0.01"
                  step="0.01"
                  placeholder="0,00"
                  required
                />
              </label>

              <label>
                <span className="field-label">
                  Дата забора <span className="required-mark">*</span>
                </span>
                <input
                  type="date"
                  name="pickupDate"
                  value={form.pickupDate}
                  onChange={updateField}
                  required
                />
              </label>
            </div>

            {error && <div className="error-message">{error}</div>}

            <button className="primary-button" disabled={isSaving}>
              {isSaving ? 'Создаем отправление...' : 'Создать отправление'}
              <span>↗</span>
            </button>
          </form>

          <section className="panel orders-panel">
            <div className="panel-heading">
              <div>
                <h3>Ваши отправления</h3>
              </div>
              <span className="order-count">{orders.length} всего</span>
            </div>

            {orders.length === 0 ? (
              <div className="empty-state">
                <div className="empty-icon">↗</div>
                <strong>Отправлений пока нет</strong>
                <span>Созданные отправления появятся здесь</span>
              </div>
            ) : (
              <div className="orders-list">
                {orders.map((order) => (
                  <button
                    className={`order-row ${selectedOrder?.id === order.id ? 'selected' : ''}`}
                    key={order.id}
                    onClick={() => setSelectedOrder(order)}
                  >
                    <span className="row-copy">
                      <strong>{order.orderNumber}</strong>
                      <small>
                        {order.senderCity} → {order.recipientCity}
                      </small>
                    </span>
                  </button>
                ))}
              </div>
            )}
          </section>
        </div>

        {selectedOrder && (
          <section className="panel detail-panel">
            <div className="panel-heading detail-heading">
              <div className="detail-title-wrap">
                <h3>Просмотр отправления</h3>
                <span className="detail-order-number">{selectedOrder.orderNumber}</span>
              </div>
            </div>

            <div className="form-section">
              <div className="section-label">МАРШРУТ</div>
              <div className="field-grid detail-field-grid">
                <div className="detail-field">
                  <span className="field-label">Город отправителя</span>
                  <strong>{selectedOrder.senderCity}</strong>
                  <span className="field-label">Адрес отправителя</span>
                  <p>{selectedOrder.senderAddress}</p>
                </div>

                <div className="detail-field">
                  <span className="field-label">Город получателя</span>
                  <strong>{selectedOrder.recipientCity}</strong>
                  <span className="field-label">Адрес получателя</span>
                  <p>{selectedOrder.recipientAddress}</p>
                </div>
              </div>
            </div>

            <div className="form-section compact-fields detail-compact-fields">
              <div className="detail-field">
                <span className="field-label">Дата забора</span>
                <strong>
                  {new Date(`${selectedOrder.pickupDate}T00:00:00`).toLocaleDateString('ru-RU')}
                </strong>
              </div>

              <div className="detail-field">
                <span className="field-label">Вес груза</span>
                <strong>{selectedOrder.weight} кг</strong>
              </div>
            </div>

          </section>
        )}
      </main>
    </div>
  )
}

export default App
