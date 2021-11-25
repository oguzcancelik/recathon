FROM python:3.9

WORKDIR /app

COPY . /app

ENV FLASK_ENV=production
ENV PYTHONDONTWRITEBYTECODE 1
ENV PYTHONUNBUFFERED 1

RUN pip install -U pip
RUN pip install uWSGI
RUN pip install -r requirements.txt

CMD ["uwsgi", "app.ini"]